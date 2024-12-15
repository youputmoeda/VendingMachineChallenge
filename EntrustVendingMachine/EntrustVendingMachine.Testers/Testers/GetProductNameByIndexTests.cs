using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class GetProductNameByIndexTests
    {
        private VendingMachineService _vendingMachine;

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

        [Test]
        public void GetProductNameByIndex_ShouldReturnCorrectProductName_WhenIndexIsValid()
        {
            // Act
            var productName = _vendingMachine.GetProductNameByIndex(1);

            // Assert
            Assert.That(productName, Is.EqualTo("Chips"));
        }

        [Test]
        public void GetProductNameByIndex_ShouldThrowArgumentOutOfRangeException_WhenIndexIsNegative()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _vendingMachine.GetProductNameByIndex(-1));
            Assert.That(exception.ParamName, Is.EqualTo("index"));
            Assert.That(exception.Message, Does.Contain("Invalid product index: -1"));
        }

        [Test]
        public void GetProductNameByIndex_ShouldThrowArgumentOutOfRangeException_WhenIndexIsOutOfBounds()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _vendingMachine.GetProductNameByIndex(3));
            Assert.That(exception.ParamName, Is.EqualTo("index"));
            Assert.That(exception.Message, Does.Contain("Invalid product index: 3"));
        }

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