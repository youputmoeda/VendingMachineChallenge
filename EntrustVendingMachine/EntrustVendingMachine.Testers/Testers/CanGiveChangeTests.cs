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
    /// Unit tests for the <see cref="VendingMachineService.CanGiveChange"/> method.
    /// </summary>
    /// <remarks>
    /// This class ensures the vending machine can properly evaluate and provide change 
    /// using various coin combinations.
    /// </remarks>
    [TestFixture]
    public class CanGiveChangeTests

    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Initializes a new instance of <see cref="VendingMachineService"/> before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
        }

        /// <summary>
        /// Verifies that the vending machine can provide exact change when sufficient coins are available.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldReturnSuccess_WhenExactChangeIsAvailable()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 5),
                new(TypesOfCoin.FiftyPence, 3)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(1.50m);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }

        /// <summary>
        /// Ensures that the vending machine fails to provide change when no coins are available.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldFail_WhenChangeIsNotAvailable()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 0),
                new(TypesOfCoin.FiftyPence, 0)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(1.50m);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Cannot give exact change. Remaining change: £1,50"));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CannotGiveChange));
        }

        /// <summary>
        /// Validates that the vending machine succeeds when no change is required.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldReturnSuccess_WhenNoChangeIsNeeded()
        {
            // Act
            var result = _vendingMachine.CanGiveChange(0m);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }

        /// <summary>
        /// Ensures the vending machine fails when a negative change value is requested.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldFail_WhenChangeIsNegative()
        {
            // Act
            var result = _vendingMachine.CanGiveChange(-0.50m);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Change must be greater than zero."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CannotGiveChange));
        }

        /// <summary>
        /// Validates that the vending machine can provide change using alternative coin combinations
        /// when a specific coin type is unavailable.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldSucess_WhenSpecificCoinIsMissing()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.TwoPound, 0),
                new(TypesOfCoin.OnePound, 2),
                new(TypesOfCoin.FiftyPence, 3)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(2m);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }

        /// <summary>
        /// Ensures that the vending machine correctly uses multiple coin types to provide change.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldSucceed_WhenMultipleCoinsAreUsed()
        {
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 2),
                new(TypesOfCoin.TwentyPence, 1),
                new(TypesOfCoin.TenPence, 1),
                new(TypesOfCoin.FivePence, 1)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(1.35m);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }

        /// <summary>
        /// Verifies that the vending machine fails to provide change when all coin stocks are empty.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldFail_WhenMachineIsEmpty()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 0),
                new(TypesOfCoin.FiftyPence, 0),
                new(TypesOfCoin.TwentyPence, 0),
                new(TypesOfCoin.TenPence, 0),
                new(TypesOfCoin.FivePence, 0),
                new(TypesOfCoin.TwoPence, 0),
                new(TypesOfCoin.OnePenny, 0)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(0.50m);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("I don't have this coin to give back your change: 50p"));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CannotGiveChange));
        }

        /// <summary>
        /// Ensures that the vending machine can provide large amounts of change when sufficient coins are available.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldSucceed_WhenLargeChangeIsPossible()
        {
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.TwoPound, 5),
                new(TypesOfCoin.OnePound, 0),
                new(TypesOfCoin.FiftyPence, 0)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(10.00m);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.True);
        }

        /// <summary>
        /// Validates that the vending machine fails when fractional change cannot be provided due to insufficient coins.
        /// </summary>
        [Test]
        public void CanGiveChange_ShouldFail_WhenChangeRequiresFractionalCoins()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 0),
                new(TypesOfCoin.FiftyPence, 0),
                new(TypesOfCoin.TwentyPence, 0),
                new(TypesOfCoin.TenPence, 0),
                new(TypesOfCoin.FivePence, 0),
                new(TypesOfCoin.TwoPence, 1),
                new(TypesOfCoin.OnePenny, 0)
            });

            // Act
            var result = _vendingMachine.CanGiveChange(0.03m);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("I don't have this coin to give back your change: 1p"));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.CannotGiveChange));
        }
    }

}