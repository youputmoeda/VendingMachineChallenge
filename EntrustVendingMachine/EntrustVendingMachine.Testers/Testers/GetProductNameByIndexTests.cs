using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.GetProductNameByIndex(int)"/> method.
    /// </summary>
    /// <remarks>
    /// This class ensures that the <see cref="VendingMachineService"/> correctly handles requests
    /// to retrieve product names by their index, including edge cases for invalid or empty indexes.
    /// </remarks>
    [TestFixture]
    public class GetProductNameByIndexTest
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Sets up a vending machine with predefined products before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Chips", 1.00m, 5),
                new Product("Candy", 0.75m, 20)
            });
        }

        /// <summary>
        /// Ensures that the correct product name is returned when the provided index is valid.
        /// </summary>
        [Test]
        public void GetProductNameByIndex_ShouldReturnCorrectProductName_WhenIndexIsValid()
        {
            // Act
            var productName = _vendingMachine.GetProductNameByIndex(1);

            // Assert
            Assert.That(productName, Is.EqualTo("Chips"));
        }

        /// <summary>
        /// Ensures that an <see cref="ArgumentOutOfRangeException"/> is thrown when the provided index is negative.
        /// </summary>
        [Test]
        public void GetProductNameByIndex_ShouldThrowArgumentOutOfRangeException_WhenIndexIsNegative()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _vendingMachine.GetProductNameByIndex(-1));
            Assert.That(exception.ParamName, Is.EqualTo("index"));
            Assert.That(exception.Message, Does.Contain("Invalid product index: -1"));
        }

        /// <summary>
        /// Ensures that an <see cref="ArgumentOutOfRangeException"/> is thrown when the provided index exceeds the product list bounds.
        /// </summary>
        [Test]
        public void GetProductNameByIndex_ShouldThrowArgumentOutOfRangeException_WhenIndexIsOutOfBounds()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _vendingMachine.GetProductNameByIndex(3));
            Assert.That(exception.ParamName, Is.EqualTo("index"));
            Assert.That(exception.Message, Does.Contain("Invalid product index: 3"));
        }

        /// <summary>
        /// Ensures that an <see cref="ArgumentOutOfRangeException"/> is thrown when trying to access a product index 
        /// from an empty vending machine.
        /// </summary>
        [Test]
        public void GetProductNameByIndex_ShouldThrowArgumentOutOfRangeException_WhenNoProductsAreLoaded()
        {
            // Arrange
            var emptyMachine = new VendingMachineService();

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => emptyMachine.GetProductNameByIndex(0));
            Assert.That(exception.ParamName, Is.EqualTo("index"));
            Assert.That(exception.Message, Does.Contain("Invalid product index: 0"));
        }
    }
}