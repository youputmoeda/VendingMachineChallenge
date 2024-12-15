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
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.InsertMoney(TypesOfCoin)"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of inserting coins into the vending machine,
    /// ensuring proper handling of scenarios such as exact payments, overpayments, invalid coins, 
    /// and insufficient funds.
    /// </remarks>
    [TestFixture]
    public class InsertMoneyTests
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Sets up the vending machine with predefined products before each test.
        /// </summary>
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

        /// <summary>
        /// Verifies that a user can complete a purchase when inserting the exact amount required.
        /// </summary>
        /// <remarks>
        /// Ensures that the machine processes exact payments correctly and updates the total value.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine returns the correct change when a user overpays.
        /// </summary>
        /// <remarks>
        /// Ensures that the machine calculates and dispenses the correct change without errors.
        /// </remarks>
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

        /// <summary>
        /// Ensures that inserting money fails if no product has been selected.
        /// </summary>
        /// <remarks>
        /// Validates that the vending machine enforces product selection before accepting payments.
        /// </remarks>
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

        /// <summary>
        /// Ensures that inserting an invalid coin type is rejected by the vending machine.
        /// </summary>
        /// <remarks>
        /// Tests the behavior of the vending machine when a non-existent or invalid coin type is provided.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine does not dispense a product when the funds are insufficient.
        /// </summary>
        /// <remarks>
        /// Ensures that the machine correctly calculates the remaining balance and rejects the transaction.
        /// </remarks>
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

        /// <summary>
        /// Ensures that the vending machine fails to process payments when it cannot provide exact change.
        /// </summary>
        /// <remarks>
        /// Validates that the machine handles overpayment scenarios where adequate change is not available.
        /// </remarks>
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