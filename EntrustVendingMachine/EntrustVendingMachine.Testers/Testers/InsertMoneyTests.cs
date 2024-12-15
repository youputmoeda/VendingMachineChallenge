using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class InsertMoneyTests
    {
        private VendingMachineService _vendingMachine;

        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Candy", 0.75m, 5)
            });
        }

        [Test]
        public void InsertMoney_ShouldAllowPurchaseWithExactAmount()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
                new Coin(TypesOfCoin.FiftyPence, 20)
            });

            var selectResult = _vendingMachine.SelectProduct("Soda");
            Assert.That(selectResult.IsSuccess, Is.True);

            // Act
            var result = _vendingMachine.InsertMoney(TypesOfCoin.OnePound);
            Assert.That(result.IsSuccess, Is.False);

            result = _vendingMachine.InsertMoney(TypesOfCoin.FiftyPence);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(1.50m));
        }

        [Test]
        public void InsertMoney_ShouldReturnChangeWhenOverpaying()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
                new Coin(TypesOfCoin.FivePence, 20),
                new Coin(TypesOfCoin.TwentyPence, 30)
            });

            var selectResult = _vendingMachine.SelectProduct("Candy");
            Assert.That(selectResult.IsSuccess, Is.True);

            // Act
            var result = _vendingMachine.InsertMoney(TypesOfCoin.TwoPound);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(TypesOfCoin.TwoPound.GetValue()));
        }

        [Test]
        public void InsertMoney_ShouldFail_WhenNoProductSelected()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10)
            });

            // Act
            var result = _vendingMachine.InsertMoney(TypesOfCoin.OnePound);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("No product selected."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.NoProductSelected));
        }

        [Test]
        public void InsertMoney_ShouldFail_WhenInvalidCoinInserted()
        {
            // Arrange
            var selectResult = _vendingMachine.SelectProduct("Candy");
            Assert.That(selectResult.IsSuccess, Is.True);

            // Act
            var result = _vendingMachine.InsertMoney((TypesOfCoin)(-1));

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Invalid coin type."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.InvalidCoinType));
        }

        [Test]
        public void InsertMoney_ShouldFail_WhenInsufficientFunds()
        {
            // Arrange
            var selectResult = _vendingMachine.SelectProduct("Soda");
            Assert.That(selectResult.IsSuccess, Is.True);

            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
            });

            // Act
            var result = _vendingMachine.InsertMoney(TypesOfCoin.OnePound);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Insufficient funds. Inserted: £1,00, Price: £1,50"));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.InsufficientFunds));
        }

        [Test]
        public void InsertMoney_ShouldFail_WhenCannotGiveChange()
        {
            // Arrange
            _vendingMachine.SelectProduct("Candy");
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
            });

            // Act
            var result = _vendingMachine.InsertMoney(TypesOfCoin.TwoPound);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Cannot give exact change. Remaining change: £0,25"));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CannotGiveChange));
        }
    }
}