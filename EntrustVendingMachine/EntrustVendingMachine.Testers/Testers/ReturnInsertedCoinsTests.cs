using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Models;
using EntrustVendingMachine.Services;

namespace EntrustVendingMachine.Testers
{
    [TestFixture]
    public class ReturnInsertedCoinsTests
    {
        private VendingMachineService _vendingMachine;

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