using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.ProductStatus"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of the product status display, ensuring accurate formatting
    /// with or without indices and totals, as well as handling scenarios where no products are loaded.
    /// </remarks>
    [TestFixture]
    public class ProductStatusTests

    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Initializes a new instance of the vending machine service and loads sample products before each test.
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
        /// Verifies that the product status displays products without indices and without showing the total value.
        /// </summary>
        /// <remarks>
        /// Ensures the product list is formatted correctly with only product names, prices, and stock counts.
        /// </remarks>
        [Test]
        public void ProductStatus_ShouldReturnProductsWithoutIndexAndWithoutTotal()
        {
            // Act
            var result = _vendingMachine.ProductStatus(includeIndex: false, includeTotal: false);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string>
            {
                "- Soda: £1,50 (Stock: 10)",
                "- Chips: £1,00 (Stock: 5)",
                "- Candy: £0,75 (Stock: 20)"
            }));
        }

        /// <summary>
        /// Verifies that the product status displays products with indices and without showing the total value.
        /// </summary>
        /// <remarks>
        /// Ensures the product list includes an index before each product entry but excludes the total value line.
        /// </remarks>
        [Test]
        public void ProductStatus_ShouldReturnProductsWithIndexAndWithoutTotal()
        {
            // Act
            var result = _vendingMachine.ProductStatus(includeIndex: true, includeTotal: false);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string>
            {
                "0) Soda: £1,50 (Stock: 10)",
                "1) Chips: £1,00 (Stock: 5)",
                "2) Candy: £0,75 (Stock: 20)"
            }));
        }

        /// <summary>
        /// Verifies that the product status displays products without indices and includes the total value of products.
        /// </summary>
        /// <remarks>
        /// Ensures the product list is formatted correctly with product details followed by the total value line.
        /// </remarks>
        [Test]
        public void ProductStatus_ShouldReturnProductsWithoutIndexAndWithTotal()
        {
            // Act
            var result = _vendingMachine.ProductStatus(includeIndex: false, includeTotal: true);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string>
            {
                "- Soda: £1,50 (Stock: 10)",
                "- Chips: £1,00 (Stock: 5)",
                "- Candy: £0,75 (Stock: 20)",
                "Total value of products: £35,00"
            }));
        }

        /// <summary>
        /// Verifies that the product status displays products with indices and includes the total value of products.
        /// </summary>
        /// <remarks>
        /// Ensures the product list is formatted correctly with product details prefixed by indices and the total value line.
        /// </remarks>
        [Test]
        public void ProductStatus_ShouldReturnProductsWithIndexAndWithTotal()
        {
            // Act
            var result = _vendingMachine.ProductStatus(includeIndex: true, includeTotal: true);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string>
            {
                "0) Soda: £1,50 (Stock: 10)",
                "1) Chips: £1,00 (Stock: 5)",
                "2) Candy: £0,75 (Stock: 20)",
                "Total value of products: £35,00"
            }));
        }

        /// <summary>
        /// Verifies that the product status correctly handles an empty machine by showing only the total value as £0.00.
        /// </summary>
        /// <remarks>
        /// Ensures no product lines are displayed when the vending machine has no loaded products.
        /// </remarks>
        [Test]
        public void ProductStatus_ShouldReturnEmptyList_WhenNoProductsAreLoaded()
        {
            // Arrange
            var emptyMachine = new VendingMachineService();

            // Act
            var result = emptyMachine.ProductStatus(includeIndex: true, includeTotal: true);

            // Assert
            Assert.That(result, Is.EquivalentTo(new List<string>
            {
                "Total value of products: £0,00"
            }));
        }
    }
}