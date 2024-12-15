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
    /// Unit tests for the <see cref="VendingMachineService.SelectProduct"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of product selection in the vending machine, including successful 
    /// selection, failure cases for invalid inputs, out-of-stock products, and nonexistent products.
    /// </remarks>
    [TestFixture]
    public class SelectProductTests
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Sets up a new instance of the vending machine with predefined products before each test.
        /// </summary>
        /// <remarks>
        /// Ensures the vending machine contains the following products:
        /// - "Soda" (Price: £1.50, Stock: 10)
        /// - "Chips" (Price: £1.00, Stock: 0)
        /// - "Candy" (Price: £0.75, Stock: 5)
        /// </remarks>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Chips", 1.00m, 0),
                new Product("Candy", 0.75m, 5)
            });
        }

        /// <summary>
        /// Verifies that selecting a valid product with available stock succeeds.
        /// </summary>
        /// <remarks>
        /// Ensures that the product "Soda" can be selected successfully, with correct price and no errors.
        /// </remarks>
        [Test]
        public void SelectProduct_ShouldSelectProductSuccessfully()
        {
            // Act
            var result = _vendingMachine.SelectProduct("Soda");

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Name, Is.EqualTo("Soda"));
            Assert.That(result.Value.Price, Is.EqualTo(1.50m));
            Assert.That(result.Error, Is.Null);
        }

        /// <summary>
        /// Verifies that attempting to select a nonexistent product fails with an appropriate error message.
        /// </summary>
        /// <remarks>
        /// This test ensures that the method returns an error when trying to select "Water," which is not in the machine.
        /// </remarks>
        [Test]
        public void SelectProduct_ShouldFail_WhenProductDoesNotExist()
        {
            // Act
            var result = _vendingMachine.SelectProduct("Water");

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("The product 'Water' does not exist."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ProductDoesNotExist));
        }

        /// <summary>
        /// Verifies that attempting to select a product that is out of stock fails with an appropriate error message.
        /// </summary>
        /// <remarks>
        /// This test checks the behavior when trying to select "Chips," which has zero stock in the vending machine.
        /// </remarks>
        [Test]
        public void SelectProduct_ShouldFail_WhenProductIsOutOfStock()
        {
            // Act
            var result = _vendingMachine.SelectProduct("Chips");

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("The product 'Chips' is out of stock."));
            Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ProductOutOfStock));
        }

        /// <summary>
        /// Verifies that attempting to select a product with an invalid name (null or whitespace) fails with an error.
        /// </summary>
        /// <remarks>
        /// Ensures the method handles invalid input gracefully and returns an appropriate error message.
        /// </remarks>
        [Test]
        public void SelectProduct_ShouldFail_WhenProductNameIsInvalid()
        {
            // Act
            var resultWithNull = _vendingMachine.SelectProduct(null);
            var resultWithWhiteSpace = _vendingMachine.SelectProduct("   ");

            // Assert
            Assert.That(resultWithNull.IsSuccess, Is.False);
            Assert.That(resultWithNull.Error, Is.EqualTo("Invalid product selection. Please try again."));
            Assert.That(resultWithNull.ErrorCode, Is.EqualTo(ErrorCode.ProductDoesNotExist));

            // Assert
            Assert.That(resultWithWhiteSpace.IsSuccess, Is.False);
            Assert.That(resultWithWhiteSpace.Error, Is.EqualTo("Invalid product selection. Please try again."));
            Assert.That(resultWithWhiteSpace.ErrorCode, Is.EqualTo(ErrorCode.ProductDoesNotExist));
        }

        /// <summary>
        /// Verifies that a product with zero stock after selection cannot be selected again.
        /// </summary>
        /// <remarks>
        /// This test ensures that once a product's stock is depleted, it becomes unavailable for further selection.
        /// The product "Gum" starts with 1 unit, is selected once, and then fails on subsequent selections.
        /// </remarks>
        [Test]
        public void SelectProduct_ShouldFail_WhenStockReachesZero()
        {
            // Arrange
            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Gum", 0.50m, 1)
            });

            var result = _vendingMachine.SelectProduct("Gum");
            Assert.That(result.IsSuccess, Is.True);

            result.Value.Stock--;

            var resultAfterStockEmpty = _vendingMachine.SelectProduct("Gum");

            // Assert
            Assert.That(resultAfterStockEmpty.IsSuccess, Is.False);
            Assert.That(resultAfterStockEmpty.Error, Is.EqualTo("The product 'Gum' is out of stock."));
            Assert.That(resultAfterStockEmpty.ErrorCode, Is.EqualTo(ErrorCode.ProductOutOfStock));
        }
    }
}