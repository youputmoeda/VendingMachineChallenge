using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    /// <summary>
    /// Unit tests for the <see cref="VendingMachineService.ReturnInsertedCoins"/> method.
    /// </summary>
    /// <remarks>
    /// These tests validate the behavior of returning inserted coins in the vending machine,
    /// ensuring proper handling of inserted coins without affecting machine coins or while adjusting
    /// coins in the machine when specified.
    /// </remarks>
    [TestFixture]
    public class ReturnInsertedCoinsTests
    {
        private VendingMachineService _vendingMachine;

        /// <summary>
        /// Sets up a new instance of the vending machine with sample products and coins before each test.
        /// </summary>
        /// <remarks>
        /// Ensures that each test starts with a predefined state for consistent results.
        /// </remarks>
        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachineService();

            _vendingMachine.LoadProductsOnMachine(new List<Product>
            {
                new Product("Soda", 1.50m, 10),
                new Product("Candy", 0.75m, 5)
            });

            _vendingMachine.LoadCoinsOnMachine(new List<Coin>
            {
                new(TypesOfCoin.OnePound, 2),
                new(TypesOfCoin.TwentyPence, 1),
                new(TypesOfCoin.TenPence, 1),
                new(TypesOfCoin.FivePence, 1)
            });
        }

        /// <summary>
        /// Verifies that returning inserted coins does not affect the coins stored in the vending machine.
        /// </summary>
        /// <remarks>
        /// This test ensures that coins inserted by the user are returned as is, 
        /// and the vending machine's coin stock remains unaffected.
        /// </remarks>
        [Test]
        public void ReturnInsertedCoins_ShouldReturnAllInsertedCoins_WithoutAffectingMachineCoins()
        {
            // Arrange
            var selectResult = _vendingMachine.SelectProduct("Soda");
            Assert.That(selectResult.IsSuccess, Is.True);

            _vendingMachine.InsertMoney(TypesOfCoin.OnePound);
            _vendingMachine.InsertMoney(TypesOfCoin.FivePence);

            // Act
            List<TypesOfCoin> returnedCoins = _vendingMachine.ReturnInsertedCoins();

            // Assert
            Assert.That(returnedCoins, Is.EquivalentTo(new[] { TypesOfCoin.OnePound, TypesOfCoin.FivePence }));

            var machineCoins = _vendingMachine.CoinsStatus();
            Assert.That(machineCoins, Does.Contain("- £1: 2 coins (Value: £2,00)")); // Confirma que as moedas não foram adicionadas
            Assert.That(machineCoins, Does.Contain("- 5p: 1 coins (Value: £0,05)")); // Confirma que as moedas não foram adicionadas
        }


        /// <summary>
        /// Verifies that returning inserted coins adjusts the vending machine's coin stock when giving change from the machine is enabled.
        /// </summary>
        /// <remarks>
        /// This test ensures that the machine deducts the appropriate coins from its stock
        /// when returning inserted coins while also giving change.
        /// </remarks>
        [Test]
        public void ReturnInsertedCoins_ShouldAdjustMachineCoins_WhenGiveChangeFromMachineIsTrue()
        {
            // Arrange
            var selectResult = _vendingMachine.SelectProduct("Soda");
            Assert.That(selectResult.IsSuccess, Is.True);

            _vendingMachine.InsertMoney(TypesOfCoin.OnePound);
            _vendingMachine.InsertMoney(TypesOfCoin.FivePence);

            // Act
            List<TypesOfCoin> returnedCoins = _vendingMachine.ReturnInsertedCoins(giveChangeFromMachine: true);

            // Assert
            Assert.That(returnedCoins, Is.EquivalentTo(new[] { TypesOfCoin.OnePound, TypesOfCoin.FivePence }));

            var machineCoins = _vendingMachine.CoinsStatus();
            Assert.That(machineCoins, Does.Contain("- £1: 1 coins (Value: £1,00)"));  // Quantidade reduzida para 1
            Assert.That(machineCoins, Does.Not.Contain("- 5p: 1 coins (Value: £0,05)")); // Todas as moedas de 50p foram retiradas
        }

        /// <summary>
        /// Verifies that returning inserted coins works correctly when no coins were inserted by the user.
        /// </summary>
        /// <remarks>
        /// This test ensures that calling the method when no coins are inserted results in an empty list being returned.
        /// </remarks>
        [Test]
        public void ReturnInsertedCoins_ShouldHandleEmptyInsertedCoins()
        {
            // Act
            List<TypesOfCoin> returnedCoins = _vendingMachine.ReturnInsertedCoins();

            // Assert
            Assert.That(returnedCoins, Is.Empty);
        }
    }

}