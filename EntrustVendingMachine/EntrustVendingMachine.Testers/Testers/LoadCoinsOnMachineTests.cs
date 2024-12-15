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
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.LoadCoinsOnMachine(List{Coin})"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of loading coins into the vending machine,
    /// ensuring proper handling of valid and invalid inputs, ordering logic, and quantity updates.
    /// </remarks>
    [TestFixture]
    public class LoadCoinsOnMachineTests
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Sets up a new instance of the vending machine service before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
        }

        /// <summary>
        /// Verifies that coins are correctly loaded into the vending machine.
        /// </summary>
        /// <remarks>
        /// Ensures that the total quantity of coins is calculated accurately.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading coins fails when the provided coin list is empty.
        /// </summary>
        /// <remarks>
        /// Validates that the vending machine rejects empty coin lists with an appropriate error message.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading coins fails when the provided coin list is null.
        /// </summary>
        /// <remarks>
        /// Validates that the vending machine returns an error when a null list is supplied.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading coins fails when one or more coins in the list are null.
        /// </summary>
        /// <remarks>
        /// Verifies that the vending machine handles invalid input by rejecting null coins in the list.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine increments the quantity of existing coins when reloaded.
        /// </summary>
        /// <remarks>
        /// Ensures that additional coins of the same type are added to the current stock.
        /// </remarks>
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

        /// <summary>
        /// Ensures that coins are ordered by their value in descending order after loading.
        /// </summary>
        /// <remarks>
        /// Verifies that the vending machine's internal state maintains correct order for coin types.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine allows loading coins with negative quantities.
        /// </summary>
        /// <remarks>
        /// Tests whether negative quantities are accepted and appropriately reflected in the total calculation.
        /// </remarks>
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

        /// <summary>
        /// Ensures that the vending machine accepts coins with a quantity of zero.
        /// </summary>
        /// <remarks>
        /// Verifies that zero-quantity coins do not cause errors and are handled gracefully.
        /// </remarks>
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