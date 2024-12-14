using System;
using EntrustVendingMachine.Models;

namespace EntrustVendingMachine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            VendingMachine vendingMachine = new VendingMachine();

            // Carregar produtos
            vendingMachine.LoadProduct(new Product("Soda", 1.50m, 10));
            vendingMachine.LoadProduct(new Product("Chips", 1.00m, 5));

            // Carregar moedas
            vendingMachine.LoadChange(new Coin(TypesOfCoin.OnePound, 10));
            vendingMachine.LoadChange(new Coin(TypesOfCoin.FiftyPence, 20));

            // Exibir produtos e moedas
            vendingMachine.ProductStatus();
            vendingMachine.CoinsStatus();

            while (true)
            {
                string productName = Console.ReadLine();
                vendingMachine.SelectProduct(productName);

                bool moneyIsAllInserted = false;
                while (!moneyIsAllInserted)
                {
                    string input = Console.ReadLine();

                    if (decimal.TryParse(input, out decimal moneyInserted))
                        moneyIsAllInserted = vendingMachine.InsertMoney(moneyInserted);
                }
            }
        }
    }
}
