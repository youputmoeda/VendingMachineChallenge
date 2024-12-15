using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class LoadCoinsOnMachineTests
    {
        private VendingMachineService _vendingMachine;

        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldLoadCoinsCorrectly()
        {
            // Arrange
            var coins = new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
                new Coin(TypesOfCoin.FiftyPence, 20)
            };

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(30));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldFail_WhenCoinListIsEmpty()
        {
            // Arrange
            var coins = new List<Coin>();

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Coin list cannot be null or empty."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CoinListEmpty));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldFail_WhenCoinListIsNull()
        {
            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(null);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Coin list cannot be null or empty."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CoinListEmpty));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldFail_WhenCoinInListIsNull()
        {
            // Arrange
            var coins = new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
                null
            };

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Coin cannot be null."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CoinListEmpty));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldIncrementQuantityForExistingCoins()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10)
            });

            var additionalCoins = new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 5)
            };

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(additionalCoins);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldOrderCoinsByValueDescending()
        {
            // Arrange
            var coins = new List<Coin>
            {
                new Coin(TypesOfCoin.FiftyPence, 10),
                new Coin(TypesOfCoin.OnePound, 5),
                new Coin(TypesOfCoin.TwentyPence, 15)
            };

            // Act
            _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            var status = _vendingMachine.CoinsStatus(includeTotal: false);
            Assert.That(status.First(), Does.Contain("£1"));
            Assert.That(status.Last(), Does.Contain("20p"));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldAllowNegativeQuantities()
        {
            // Arrange
            var coins = new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, -10)
            };

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(-10));
        }

        [Test]
        public void LoadCoinsOnMachine_ShouldAcceptZeroQuantity()
        {
            // Arrange
            var coins = new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 0)
            };

            // Act
            var result = _vendingMachine.LoadCoinsOnMachine(coins);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(0));
        }
    }
}