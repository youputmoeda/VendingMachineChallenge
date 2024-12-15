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
    public class SelectProductTests
    {
        private VendingMachineService _vendingMachine;

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