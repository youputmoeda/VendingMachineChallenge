using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Utilities;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Services
{
    /// <summary>
    /// Represents the service for managing a vending machine's operations, including product and coin management, purchasing, and change calculations.
    /// </summary>
    public class VendingMachineService
    {
        private Dictionary<string, Product> _products = new();
        private Dictionary<TypesOfCoin, Coin> _coinsOnMachine = new();
        private List<TypesOfCoin> _coinsInserted = new();
        private Product? _selectedProducted;

        /// <summary>
        /// Loads a list of products into the vending machine.
        /// </summary>
        /// <param name="products">The list of products to load.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the number of successfully loaded products or an error message if the operation fails.
        /// </returns>
        public Result<int> LoadProductsOnMachine(List<Product> products)
        {
            if (products == null || products.Count == 0)
                return Result<int>.Fail("Product list cannot be null or empty.", ErrorCode.ProductListEmpty);

            int loadedProducts = 0;
            foreach (var product in products)
            {
                if (product == null)
                    return Result<int>.Fail("One or more products are null.", ErrorCode.ProductListEmpty);

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

        /// <summary>
        /// Removes a list of products from the vending machine.
        /// </summary>
        /// <param name="products">The list of products to remove.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the number of successfully removed products or an error message if the operation fails.
        /// </returns>
        public Result<int> UnloadProductsFromMachine(List<Product> products)
        {
            if (products == null || products.Count == 0)
                return Result<int>.Fail("Product list cannot be null or empty.", ErrorCode.ProductListEmpty);

            int unloadedProducts = 0;

            foreach (var product in products)
            {
                if (product == null)
                    return Result<int>.Fail("Product cannot be null.", ErrorCode.ProductListEmpty);

                if (_products.TryGetValue(product.Name, out Product? machineProduct))
                {
                    if (machineProduct.Stock < product.Stock)
                    {
                        // Notify the user of insufficient stock
                        Console.WriteLine($"Requested {product.Stock} units of '{product.Name}', but only {machineProduct.Stock} available.");
                        Console.WriteLine($"Would you like to remove all {machineProduct.Stock} units? (yes/no)");

                        string response = Console.ReadLine()?.Trim().ToLower();
                        if (response == "yes")
                        {
                            unloadedProducts += machineProduct.Stock;
                            _products.Remove(product.Name);
                        }
                        else
                        {
                            Console.WriteLine($"Skipped removing '{product.Name}'.");
                        }
                    }
                    else
                    {
                        machineProduct.Stock -= product.Stock;
                        unloadedProducts += product.Stock;

                        if (machineProduct.Stock == 0)
                        {
                            _products.Remove(product.Name);
                            Console.WriteLine($"'{product.Name}' stock is now empty and was removed from the machine.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The machine does not have the product '{product.Name}'.");
                }
            }

            return Result<int>.Success(unloadedProducts);
        }

        /// <summary>
        /// Loads a list of coins into the vending machine.
        /// </summary>
        /// <param name="coins">The list of coins to load.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the number of successfully loaded coins or an error message if the operation fails.
        /// </returns>
        public Result<int> LoadCoinsOnMachine(List<Coin> coins)
        {
            if (coins == null || coins.Count == 0)
                return Result<int>.Fail("Coin list cannot be null or empty.", ErrorCode.CoinListEmpty);

            int loadedCoins = 0;
            foreach (var coin in coins)
            {
                if (coin == null)
                    return Result<int>.Fail("Coin cannot be null.", ErrorCode.CoinListEmpty);

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

        /// <summary>
        /// Unloads a list of coins from the vending machine.
        /// </summary>
        /// <param name="coins">The list of coins to unload.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the number of successfully unloaded coins or an error message if the operation fails.
        /// </returns>
        public Result<int> UnloadCoinsFromMachine(List<Coin> coins)
        {
            if (coins == null || coins.Count == 0)
                return Result<int>.Fail("Coin list cannot be null or empty.", ErrorCode.CoinListEmpty);

            int unloadedCoins = 0;

            foreach (var coin in coins)
            {
                if (coin == null)
                    return Result<int>.Fail("Coin cannot be null.", ErrorCode.CoinListEmpty);

                if (_coinsOnMachine.TryGetValue(coin.CoinType, out Coin? machineCoin))
                {
                    if (machineCoin.Quantity < coin.Quantity)
                    {
                        Console.WriteLine($"Requested {coin.Quantity} coins of {coin.CoinType.GetName()}, but only {machineCoin.Quantity} available.");
                        Console.WriteLine($"Would you like to remove all {machineCoin.Quantity} coins? (yes/no)");

                        string response = Console.ReadLine()?.Trim().ToLower();
                        if (response == "yes")
                        {
                            unloadedCoins = machineCoin.Quantity;
                            machineCoin.Quantity = 0;
                        }
                        else
                        {
                            Console.WriteLine($"Skipped removing {coin.CoinType.GetName()} coins.");
                        }
                    }
                    else
                    {
                        machineCoin.Quantity -= coin.Quantity;
                        unloadedCoins += coin.Quantity;
                    }
                }
                else
                {
                    Console.WriteLine($"The machine does not have any {coin.CoinType.GetName()} coins.");
                }
            }

            return Result<int>.Success(unloadedCoins);
        }

        /// <summary>
        /// Selects a product from the vending machine based on the product name.
        /// </summary>
        /// <param name="product">The name of the product to select.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the selected product if successful, or an error message if the operation fails.
        /// </returns>
        public Result<Product> SelectProduct(string product)
        {
            if (string.IsNullOrWhiteSpace(product))
                return Result<Product>.Fail("Invalid product selection. Please try again.", ErrorCode.ProductDoesNotExist);

            if (!_products.TryGetValue(product, out var selectedProduct))
                return Result<Product>.Fail($"The product '{product}' does not exist.", ErrorCode.ProductDoesNotExist);

            if (selectedProduct.Stock <= 0)
                return Result<Product>.Fail($"The product '{product}' is out of stock.", ErrorCode.ProductOutOfStock);

            _selectedProducted = selectedProduct;
            return Result<Product>.Success(_selectedProducted);
        }

        /// <summary>
        /// Inserts money into the vending machine.
        /// </summary>
        /// <param name="userChange">The type of coin being inserted.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the total inserted value if successful, or an error message if the operation fails.
        /// </returns>
        public Result<decimal> InsertMoney(TypesOfCoin userChange)
        {
            if (!Enum.IsDefined(typeof(TypesOfCoin), userChange))
                return Result<decimal>.Fail("Invalid coin type.", ErrorCode.InvalidCoinType);


            if (_selectedProducted == null)
                return Result<decimal>.Fail("No product selected.", ErrorCode.NoProductSelected);

            _coinsInserted.Add(userChange);

            decimal total = _coinsInserted.Sum(coin => coin.GetValue());

            if (total < _selectedProducted.Price)
                return Result<decimal>.FailWithValue(
                    $"Insufficient funds. Inserted: £{total:F2}, Price: £{_selectedProducted.Price:F2}",
                    value: total,
                    ErrorCode.InsufficientFunds
                );

            var loadResult = LoadCoinsOnMachine(
                _coinsInserted.GroupBy(coin => coin)
                              .Select(group => new Coin(group.Key, group.Count()))
                              .ToList()
            );

            if (!loadResult.IsSuccess)
            {
                Console.WriteLine($"Error loading coins into machine: {loadResult.Error}");
                ReturnInsertedCoins();
                return Result<decimal>.Fail("Error loading coins into machine.", ErrorCode.CoinLoadingError);
            }

            //Because loading the coins from the user was a sucess, if something went error we need to give his coins back.
            Result<bool> canGiveChangeResult = CanGiveChange(total - _selectedProducted.Price);
            if (!canGiveChangeResult.IsSuccess)
            {
                ReturnInsertedCoins(giveChangeFromMachine: true);
                return Result<decimal>.Fail(canGiveChangeResult.Error, ErrorCode.CannotGiveChange);
            }

            Result<List<Coin>> giveChangeResult = GiveTheChange(total - _selectedProducted.Price);
            if (!giveChangeResult.IsSuccess)
            {
                Console.WriteLine($"Error giving change: {giveChangeResult.Error}");
                ReturnInsertedCoins(giveChangeFromMachine: true);
                return Result<decimal>.Fail(giveChangeResult.Error, ErrorCode.CannotGiveChange);
            }


            Result<Product> giveProductResult = GiveTheProduct();
            if (!giveProductResult.IsSuccess)
            {
                Console.WriteLine($"Error delivering product: {giveProductResult.Error}");
                ReturnInsertedCoins(giveChangeFromMachine: true);
                return Result<decimal>.Fail("Error delivering product.", ErrorCode.ProductDeliveryError);
            }

            return Result<decimal>.Success(total);
        }

        /// <summary>
        /// Checks if the machine can provide the specified change.
        /// </summary>
        /// <param name="change">The amount of change to be given.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> indicating success if exact change can be given, or an error message if it cannot.
        /// </returns>
        public Result<bool> CanGiveChange(decimal change)
        {
            if (change < 0)
                return Result<bool>.Fail("Change must be greater than zero.", ErrorCode.CannotGiveChange);

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
            {
                var missingCoin = Enum.GetValues(typeof(TypesOfCoin))
                                      .Cast<TypesOfCoin>()
                                      .Where(coin => coin.GetValue() == change)
                                      .OrderByDescending(coin => coin.GetValue())
                                      .FirstOrDefault();

                if (missingCoin == default(TypesOfCoin))
                {
                    return Result<bool>.Fail($"Cannot give exact change. Remaining change: £{change:F2}", ErrorCode.CannotGiveChange);
                }

                return Result<bool>.Fail($"I don't have this coin to give back your change: {missingCoin.GetName()}", ErrorCode.CannotGiveChange);
            }

            return Result<bool>.Success(change == 0);
        }

        /// <summary>
        /// Dispenses the selected product to the user.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the dispensed product if successful, 
        /// or an error message if the operation fails.
        /// </returns>
        /// <remarks>
        /// This method reduces the stock of the selected product by one. If no product is selected, 
        /// it returns a failure result. The method also clears the user's inserted coins.
        /// </remarks>
        private Result<Product> GiveTheProduct()
        {
            if (_selectedProducted == null)
                return Result<Product>.Fail("No product selected.", ErrorCode.NoProductSelected);

            Console.WriteLine("Processing Product......");
            _products[_selectedProducted.Name].Stock--;
            Console.WriteLine($"{_selectedProducted.Name} Coming out");

            Product deliveredProduct = _selectedProducted;
            _selectedProducted = null;
            _coinsInserted.Clear();

            return Result<Product>.Success(deliveredProduct);
        }
        
        /// <summary>
        /// Dispenses the specified amount of change to the user using the available coins in the machine.
        /// </summary>
        /// <param name="change">The amount of change to be dispensed.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing a list of dispensed coins if successful,
        /// or an error message if the exact change cannot be given.
        /// </returns>
        /// <remarks>
        /// The method iterates through the coins in the machine in descending order of value, 
        /// dispensing coins until the required change is met. If the machine cannot provide 
        /// the exact change, it returns a failure result.
        /// </remarks>
        private Result<List<Coin>> GiveTheChange(decimal change)
        {
            if (change <= 0)
                return Result<List<Coin>>.Success(new List<Coin>());

            Console.WriteLine($"Processing Change......£{change}");

            List<Coin> givenChange = new();

            foreach (var coin in _coinsOnMachine.OrderByDescending(c => c.Value.GetValue()))
            {
                while (coin.Value.Quantity > 0 && change >= coin.Value.CoinType.GetValue())
                {
                    change -= coin.Value.CoinType.GetValue();
                    change = Math.Round(change, 2);
                    coin.Value.Quantity--;
                    givenChange.Add(new Coin(coin.Key, 1));
                    Console.WriteLine(coin.Value.CoinType.GetName());
                }
            }

            if (change != 0)
                return Result<List<Coin>>.Fail("Unable to give exact change.", ErrorCode.CannotGiveChange);

            return Result<List<Coin>>.Success(givenChange);
        }

        /// <summary>
        /// Returns all coins inserted by the user.
        /// </summary>
        /// <param name="giveChangeFromMachine">Indicates whether the machine's coins should be used to give change.</param>
        /// <returns>A list of coins returned to the user.</returns>
        public List<TypesOfCoin> ReturnInsertedCoins(bool giveChangeFromMachine = false)
        {
            if (!_coinsInserted.Any())
            {
                Console.WriteLine("No coins to return.");
                return new List<TypesOfCoin>();
            }

            if (giveChangeFromMachine)
            {
                HandleGiveChangeFromMachineUsingUnload();
            }

            Console.WriteLine("Returning inserted coins:");
            foreach (TypesOfCoin coin in _coinsInserted)
                Console.WriteLine($"- {coin.GetName()} (£{coin.GetValue():F2})");

            List<TypesOfCoin> returnedCoins = new List<TypesOfCoin>(_coinsInserted);
            _coinsInserted.Clear();
            return returnedCoins;
        }

        private void HandleGiveChangeFromMachineUsingUnload()
        {
            var groupedCoins = _coinsInserted
                .GroupBy(c => c)
                .Select(g => new Coin(g.Key, g.Count()))
                .ToList();

            foreach (var coin in groupedCoins)
            {
                var unloadResult = UnloadCoinsFromMachine(new List<Coin> { coin });

                if (!unloadResult.IsSuccess)
                {
                    Console.WriteLine($"Warning: Could not fully unload {coin.Quantity} {coin.CoinType.GetName()} from the machine.");
                    Console.WriteLine($"Reason: {unloadResult.Error}");
                }
            }
        }

        /// <summary>
        /// Provides a detailed status of the products in the machine.
        /// </summary>
        /// <param name="includeIndex">Whether to include an index for each product.</param>
        /// <param name="includeTotal">Whether to include the total value of all products.</param>
        /// <returns>A list of strings describing the status of products in the machine.</returns>
        public List<string> ProductStatus(bool includeIndex = false, bool includeTotal = false)
        {
            List<string> status = new List<string>();

            foreach (var (product, index) in _products.Values.Select((p, i) => (p, i)))
            {
                string line = includeIndex
                    ? $"{index}) {product.Name}: £{product.Price:F2} (Stock: {product.Stock})"
                    : $"- {product.Name}: £{product.Price:F2} (Stock: {product.Stock})";

                status.Add(line);
            }

            if (includeTotal)
            {
                decimal totalValue = _products.Values.Sum(p => p.Price * p.Stock);
                status.Add($"Total value of products: £{totalValue:F2}");
            }

            return status;
        }

        /// <summary>
        /// Gets the name of a product by its index.
        /// </summary>
        /// <param name="index">The index of the product.</param>
        /// <returns>The name of the product at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the index is out of range or no products are loaded.
        /// </exception>
        public string GetProductNameByIndex(int index)
        {
            if (index < 0 || index >= _products.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Invalid product index: {index}. Must be between 0 and {_products.Count - 1}.");

            return _products.ElementAt(index).Key;
        }

        /// <summary>
        /// Provides a detailed status of the coins in the machine.
        /// </summary>
        /// <param name="includeTotal">Whether to include the total value of all coins.</param>
        /// <returns>A list of strings describing the status of coins in the machine.</returns>
        public List<string> CoinsStatus(bool includeTotal = true)
        {
            if (!_coinsOnMachine.Any())
                return new List<string> { "No coins available in the machine." };
            var status = new List<string>();

            foreach (var coin in _coinsOnMachine.Values)
            {
                if (coin.Quantity > 0)
                    status.Add($"- {coin.CoinType.GetName()}: {coin.Quantity} coins (Value: £{coin.CoinType.GetValue() * coin.Quantity:F2})");
            }

            if (includeTotal)
            {
                decimal totalMoney = _coinsOnMachine.Values.Sum(c => c.CoinType.GetValue() * c.Quantity);
                status.Add($"Total money in machine: £{totalMoney:F2}");
            }

            return status;
        }
    }
}