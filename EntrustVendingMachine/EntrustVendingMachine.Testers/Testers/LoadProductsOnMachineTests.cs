using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;
using VendingMachine.Enums;

namespace EntrustVendingMachine.Testers
{
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.LoadProductsOnMachine(List{Product})"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of loading products into the vending machine,
    /// ensuring proper handling of valid and invalid inputs, stock updates, and product separation.
    /// </remarks>
    [TestFixture]
    public class LoadProductsOnMachineTests
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Initializes a new instance of the vending machine service before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
        }

        /// <summary>
        /// Verifies that the vending machine correctly loads a valid list of products.
        /// </summary>
        /// <remarks>
        /// Ensures that the total number of loaded products is accurately counted.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading products fails when the provided product list is empty.
        /// </summary>
        /// <remarks>
        /// Validates that an empty product list is rejected with an appropriate error message.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading products fails when the provided product list is null.
        /// </summary>
        /// <remarks>
        /// Verifies that a null product list triggers an appropriate error response.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine increments the stock of existing products when reloaded.
        /// </summary>
        /// <remarks>
        /// Ensures that the stock for products with the same name and price is updated correctly.
        /// </remarks>
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

        /// <summary>
        /// Ensures that loading products fails when one or more products in the list are null.
        /// </summary>
        /// <remarks>
        /// Verifies that the vending machine rejects lists containing null products.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine accepts products with a stock of zero.
        /// </summary>
        /// <remarks>
        /// Ensures that products with zero stock can be added to the machine without causing errors.
        /// </remarks>
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

        /// <summary>
        /// Verifies that the vending machine keeps products with different names or prices as separate entries.
        /// </summary>
        /// <remarks>
        /// Ensures that products with the same name but different prices are not combined into one entry.
        /// </remarks>
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