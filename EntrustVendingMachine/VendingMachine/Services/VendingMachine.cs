using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Utilities;

namespace EntrustVendingMachine.Services
{
    public class VendingMachineService
    {
        private Dictionary<string, Product> _products = new();
        private Dictionary<TypesOfCoin, Coin> _coinsOnMachine = new();

        private List<TypesOfCoin> _coinsInserted = new();

        private Product? _selectedProducted;

        public Result<int> LoadProductsOnMachine(List<Product> products)
        {
            if (products == null || products.Count == 0)
                return Result<int>.Fail("Product list cannot be null or empty.");

            int loadedProducts = 0;
            foreach (var product in products)
            {
                if (product == null)
                    return Result<int>.Fail("One or more products are null.");

                if (!_products.TryGetValue(product.Name, out Product? existingProduct))
                {
                    _products[product.Name] = product;
                }
                else
                {
                    existingProduct.Stock += product.Stock;
                }

                loadedProducts++;
            }

            return Result<int>.Success(loadedProducts);
        }

        public Result<int> LoadCoinsOnMachine(List<Coin> coins)
        {
            if (coins == null || coins.Count == 0)
                return Result<int>.Fail("Change list cannot be null or empty.");

            int loadedCoins = 0;
            foreach (var coin in coins)
            {
                if (coin == null)
                    throw new ArgumentException("Coin cannot be null.", nameof(coins));

                if (_coinsOnMachine.TryGetValue(coin.CoinType, out Coin? value))
                {
                    value.Quantity += coin.Quantity;
                }
                else
                {
                    _coinsOnMachine[coin.CoinType] = coin;
                }
                loadedCoins += coin.Quantity;
            }
            _coinsOnMachine = _coinsOnMachine
                .OrderByDescending(coin => coin.Value.GetValue())
                .ToDictionary(coin => coin.Key, coin => coin.Value);

            return Result<int>.Success(loadedCoins);
        }

        public Result<Product> SelectProduct(string product)
        {
            if (string.IsNullOrWhiteSpace(product))
                return Result<Product>.Fail("Invalid product selection. Please try again.");

            if (!_products.TryGetValue(product, out var selectedProduct))
                return Result<Product>.Fail($"The product '{product}' does not exist.");

            if (selectedProduct.Stock <= 0)
                return Result<Product>.Fail($"The product '{product}' is out of stock.");

            _selectedProducted = selectedProduct;
            return Result<Product>.Success(_selectedProducted);
        }

        public Result<decimal> InsertMoney(TypesOfCoin userChange)
        {
            if (_selectedProducted == null)
                return Result<decimal>.Fail("No product selected.");

            if (userChange == null)
                return Result<decimal>.Fail("Invalid coin type.");

            _coinsInserted.Add(userChange);

            decimal total = _coinsInserted.Sum(coin => coin.GetValue());
            Console.WriteLine($"Credit: £{total:F2}");

            if (total <= _selectedProducted.Price)
                return Result<decimal>.Success(total);

            Result<bool> canGiveChangeResult = CanGiveChange(total - _selectedProducted.Price);
            if (!canGiveChangeResult.IsSuccess)
            {
                ReturnInsertedCoins();
                return Result<decimal>.Fail(canGiveChangeResult.Error);
            }

            var loadResult = LoadCoinsOnMachine(
                _coinsInserted.GroupBy(coin => coin)
                              .Select(group => new Coin(group.Key, group.Count()))
                              .ToList()
            );

            if (!loadResult.IsSuccess)
            {
                Console.WriteLine($"Error loading coins into machine: {loadResult.Error}");
                ReturnInsertedCoins();
                return Result<decimal>.Fail("Error loading coins into machine. Returning inserted coins.");
            }

            Result<List<Coin>> giveChangeResult = GiveTheChange(total - _selectedProducted.Price);
            if (!giveChangeResult.IsSuccess)
            {
                Console.WriteLine($"Error giving change: {giveChangeResult.Error}");
                ReturnInsertedCoins();
                return Result<decimal>.Fail("Error giving change. Returning inserted coins.");
            }
            

            Result<Product> giveProductResult = GiveTheProduct();
            if (!giveProductResult.IsSuccess)
            {
                Console.WriteLine($"Error delivering product: {giveProductResult.Error}");
                ReturnInsertedCoins(); // Devolver moedas inseridas em caso de erro ao entregar o produto
                return Result<decimal>.Fail("Error delivering product. Returning inserted coins.");
            }

            return Result<decimal>.Success(total);
        }

        public Result<bool> CanGiveChange(decimal change)
        {
            if (change <= 0)
                return Result<bool>.Fail("Change must be greater than zero.");

            Dictionary<TypesOfCoin, int>? coinTemp = _coinsOnMachine.ToDictionary(
                coin => coin.Key,
                coin => coin.Value.Quantity
            );

            foreach (var coin in _coinsOnMachine.OrderByDescending(c => c.Value.GetValue()))
            {
                while (coinTemp[coin.Key] > 0 && change >= coin.Value.CoinType.GetValue())
                {
                    change -= coin.Value.CoinType.GetValue();
                    change = Math.Round(change, 2);
                    coinTemp[coin.Key]--;
                }
            }

            if (change != 0)
                return Result<bool>.Fail($"I don't have this coin to give back your change: " +
                    $"{Enum.GetValues(typeof(TypesOfCoin))
                          .Cast<TypesOfCoin>()
                          .FirstOrDefault(coin => coin.GetValue() == change).GetName()}");

            return Result<bool>.Success(change != 0);
        }

        private Result<Product> GiveTheProduct()
        {
            if (_selectedProducted == null)
                return Result<Product>.Fail("No product selected.");

            Console.WriteLine("Processing Product......");
            _products[_selectedProducted.Name].Stock--;
            Console.WriteLine($"{_selectedProducted.Name} Coming out");

            Product deliveredProduct = _selectedProducted;
            _selectedProducted = null;
            _coinsInserted.Clear();

            return Result<Product>.Success(deliveredProduct);
        }

        private Result<List<Coin>> GiveTheChange(decimal change)
        {
            if (change <= 0)
                return Result<List<Coin>>.Success(new List<Coin>());

            Console.WriteLine("Processing Change......");

            List<Coin> givenChange = new();

            foreach (var coin in _coinsOnMachine.OrderByDescending(c => c.Value.GetValue()))
            {
                while (coin.Value.Quantity > 0 && change >= coin.Value.CoinType.GetValue())
                {
                    change -= coin.Value.CoinType.GetValue();
                    change = Math.Round(change, 2);
                    coin.Value.Quantity--;
                    givenChange.Add(new Coin(coin.Key, 1));
                }
            }

            return Result<List<Coin>>.Success(givenChange);
        }

        public List<TypesOfCoin> ReturnInsertedCoins()
        {
            List<TypesOfCoin> returnedCoins = new List<TypesOfCoin>(_coinsInserted);

            Console.WriteLine("Returning inserted coins:");

            foreach (TypesOfCoin coin in returnedCoins)
                Console.WriteLine($"- {coin.GetName()} (£{coin.GetValue():F2})");
            
            _coinsInserted.Clear();
            return returnedCoins;
        }

        public List<string> ProductStatus()
        {
            List<string> status = new List<string>();

            foreach (Product product in _products.Values)
            {
                status.Add($"- {product.Name}: £{product.Price} (Stock: {product.Stock})");
            }

            return status;
        }

        public List<string> CoinsStatus()
        {
            var status = new List<string>();

            foreach (var coin in _coinsOnMachine.Values)
            {
                status.Add($"- {coin.CoinType.GetName()}: {coin.Quantity} coins (Value: £{coin.CoinType.GetValue() * coin.Quantity:F2})");
            }

            return status;
        }
    }
}