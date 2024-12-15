using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class ProductStatusTests
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