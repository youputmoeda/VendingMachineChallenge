using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Testers
{
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.CoinsStatus"/> method.
    /// </summary>
    /// <remarks>
    /// This class validates the status reporting of coins in the vending machine, including 
    /// scenarios where coins are available, unavailable, or have zero quantities.
    /// </remarks>
    [TestFixture]
    public class CoinsStatusTests
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
        /// Ensures that the vending machine reports a message when no coins are available.
        /// </summary>
        [Test]
        public void CoinsStatus_ShouldReturnMessage_WhenNoCoinsAreAvailable()
        {
            // Act
            var status = _vendingMachine.CoinsStatus();

            // Assert
            Assert.That(status.Count, Is.EqualTo(1));
            Assert.That(status[0], Is.EqualTo("No coins available in the machine."));
        }

        /// <summary>
        /// Validates the coin status when a single type of coin is loaded into the vending machine.
        /// </summary>
        [Test]
        public void CoinsStatus_ShouldReturnCorrectStatus_ForSingleCoinType()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 5)
            });

            // Act
            var status = _vendingMachine.CoinsStatus();

            // Assert
            Assert.That(status.Count, Is.EqualTo(2)); // One coin line + total
            Assert.That(status[0], Is.EqualTo("- £1: 5 coins (Value: £5,00)"));
            Assert.That(status[1], Is.EqualTo("Total money in machine: £5,00"));
        }

        /// <summary>
        /// Verifies that the vending machine correctly reports the status for multiple coin types.
        /// </summary>
        [Test]
        public void CoinsStatus_ShouldReturnCorrectStatus_ForMultipleCoinTypes()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 10),
                new Coin(TypesOfCoin.FiftyPence, 20)
            });

            // Act
            var status = _vendingMachine.CoinsStatus();

            // Assert
            Assert.That(status.Count, Is.EqualTo(3)); // Two coin lines + total
            Assert.That(status[0], Is.EqualTo("- £1: 10 coins (Value: £10,00)"));
            Assert.That(status[1], Is.EqualTo("- 50p: 20 coins (Value: £10,00)"));
            Assert.That(status[2], Is.EqualTo("Total money in machine: £20,00"));
        }

        /// <summary>
        /// Ensures that coins with a zero quantity are excluded from the vending machine's status report.
        /// </summary>
        [Test]
        public void CoinsStatus_ShouldExcludeCoinsWithZeroQuantity()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 0),
                new Coin(TypesOfCoin.FiftyPence, 10)
            });

            // Act
            var status = _vendingMachine.CoinsStatus();

            // Assert
            Assert.That(status.Count, Is.EqualTo(2)); // One coin line + total
            Assert.That(status[0], Is.EqualTo("- 50p: 10 coins (Value: £5,00)"));
            Assert.That(status[1], Is.EqualTo("Total money in machine: £5,00"));
        }

        /// <summary>
        /// Validates that the vending machine calculates the total money correctly based on loaded coins.
        /// </summary>
        [Test]
        public void CoinsStatus_ShouldReturnCorrectTotalMoney()
        {
            // Arrange
            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new Coin(TypesOfCoin.OnePound, 3),
                new Coin(TypesOfCoin.TwentyPence, 10)
            });

            // Act
            var status = _vendingMachine.CoinsStatus();

            // Assert
            Assert.That(status.Last(), Is.EqualTo("Total money in machine: £5,00"));
        }
    }
}