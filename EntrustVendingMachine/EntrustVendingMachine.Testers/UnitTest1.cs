using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    public class Tests
    {
        private VendingMachineService _vendingMachine;

        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();

            // Carrega produtos na máquina
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new("Soda", 1.50m, 10),
                new("Chips", 1.00m, 5)
            });

            // Carrega moedas na máquina
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 10),
                new(TypesOfCoin.FiftyPence, 20)
            });
        }

        [Test]
        public void TestLoadProducts()
        {
            var result = _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new("Candy", 0.75m, 20)
            });

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value);
        }

        [Test]
        public void TestLoadCoins()
        {
            var result = _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.TwentyPence, 30)
            });

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(30, result.Value); // 30 moedas carregadas
        }

        [Test]
        public void TestSelectProduct_Success()
        {
            var result = _vendingMachine.SelectProduct("Soda");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Soda", result.Value.Name);
            Assert.AreEqual(1.50m, result.Value.Price);
        }

        [Test]
        public void TestSelectProduct_Failure()
        {
            var result = _vendingMachine.SelectProduct("NonExistentProduct");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("The product 'NonExistentProduct' does not exist.", result.Error);
        }

        [Test]
        public void TestInsertMoney_ExactAmount()
        {
            _vendingMachine.SelectProduct("Soda");
            var result = _vendingMachine.InsertMoney(TypesOfCoin.OnePound);
            Assert.IsFalse(result.IsSuccess); // Dinheiro ainda insuficiente

            result = _vendingMachine.InsertMoney(TypesOfCoin.FiftyPence);
            Assert.IsTrue(result.IsSuccess); // Valor exato
        }

        [Test]
        public void TestInsertMoney_ChangeRequired()
        {
            _vendingMachine.SelectProduct("Soda");
            _vendingMachine.InsertMoney(TypesOfCoin.TwoPound);

            var coinStatus = _vendingMachine.CoinsStatus();
            Assert.IsNotEmpty(coinStatus); // Confirma que as moedas foram carregadas corretamente
        }

        [Test]
        public void TestCannotGiveChange()
        {
            _vendingMachine.SelectProduct("Soda");

            // Remove todas as moedas de 50p
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.FiftyPence, -20) // Força a remoção
            });

            var result = _vendingMachine.InsertMoney(TypesOfCoin.TwoPound);

            Assert.IsFalse(result.IsSuccess); // Sem troco possível
            Assert.AreEqual("Unable to provide exact change. Returning coins.", result.Error);
        }
    }
}