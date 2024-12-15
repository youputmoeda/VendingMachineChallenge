using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class LoadProductsOnMachineTests
    {
        private VendingMachineService _vendingMachine;

        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
        }

        [Test]
        public void LoadProductsOnMachine_ShouldLoadProductsCorrectly()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Chips", 1.00m, 5)
            };

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(products);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(2));
        }

        [Test]
        public void LoadProductsOnMachine_ShouldFail_WhenProductListIsEmpty()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(products);

            // Assert
            Assert.That(result.Error, Is.EqualTo("Product list cannot be null or empty."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ProductListEmpty));
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void LoadProductsOnMachine_ShouldFail_WhenProductListIsNull()
        {
            // Act
            var result = _vendingMachine.LoadProductsOnMachine(null);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("Product list cannot be null or empty."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ProductListEmpty));
        }

        [Test]
        public void LoadProductsOnMachine_ShouldIncrementStockForExistingProducts()
        {
            // Arrange
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Soda", 1.50m, 10)
            });

            var additionalProducts = new List<Product>
            {
                new Product("Soda", 1.50m, 5)
            };

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(additionalProducts);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(1)); // Um produto foi processado
        }

        [Test]
        public void LoadProductsOnMachine_ShouldFail_WhenProductInListIsNull()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                null
            };

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(products);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("One or more products are null."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ProductListEmpty));
        }

        [Test]
        public void LoadProductsOnMachine_ShouldAcceptProductsWithZeroStock()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product("Candy", 0.75m, 0)
            };

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(products);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(1));
        }

        [Test]
        public void LoadProductsOnMachine_ShouldKeepProductsWithDifferentPricesAndNamesSeparate()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Chips", 1.00m, 5)
            };

            // Act
            var result = _vendingMachine.LoadProductsOnMachine(products);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(2));
        }
    }
}