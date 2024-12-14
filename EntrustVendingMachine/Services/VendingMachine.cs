using System;
using System.Collections.Generic;
using EntrustVendingMachine.Extensions;

namespace EntrustVendingMachine.Models
{
    public class VendingMachine
    {
        private readonly Dictionary<string, Product> _products = new();
        private readonly Dictionary<TypesOfCoin, Coin> _coins = new();

        public void LoadProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            if (_products.ContainsKey(product.Name))
            {
                _products[product.Name].Stock += product.Stock;
            }
            else
            {
                _products[product.Name] = product;
            }
        }

        public void LoadChange(Coin change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change), "Coin cannot be null.");

            if (_coins.ContainsKey(change.CoinType))
            {
                _coins[change.CoinType].Quantity += change.Quantity;
            }
            else
            {
                _coins[change.CoinType] = change;
            }
        }

        public void ProductStatus()
        {
            Console.WriteLine("Available products:");
            foreach (var product in _products.Values)
            {
                Console.WriteLine($"- {product.Name}: £{product.Price} (Stock: {product.Stock})");
            }
        }

        public void CoinsStatus()
        {
            Console.WriteLine("Available coins:");
            foreach (var coin in _coins.Values)
            {
                Console.WriteLine($"- {coin.CoinType.GetName()}: {coin.Quantity} coins (Value: £{coin.CoinType.GetValue()})");
            }
        }
    }
}
