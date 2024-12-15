using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntrustVendingMachine.Models
{
    /// <summary>
    /// Represents a coin used in the vending machine.
    /// </summary>
    public class Coin
    {
        /// <summary>
        /// Gets or sets the type of the coin.
        /// </summary>
        public TypesOfCoin CoinType { get; set; }

        /// <summary>
        /// Gets or sets the quantity of this coin in the vending machine.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Coin"/> class.
        /// </summary>
        /// <param name="coinType">The type of the coin.</param>
        /// <param name="quantity">The quantity of the coin.</param>
        public Coin(TypesOfCoin coinType, int quantity)
        {
            CoinType = coinType;
            Quantity = quantity;
        }

        /// <summary>
        /// Gets the name of the coin.
        /// </summary>
        /// <returns>A string representing the name of the coin (e.g., "£1", "50p").</returns>
        public string GetName()
        {
            return CoinType.GetName();
        }

        /// <summary>
        /// Gets the value of the coin.
        /// </summary>
        /// <returns>A decimal representing the monetary value of the coin.</returns>
        public decimal GetValue()
        {
            return CoinType.GetValue();
        }
    }
}
