using EntrustVendingMachine.Enums;
using System;

namespace EntrustVendingMachine.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="TypesOfCoin"/> enumeration.
    /// </summary>
    public static class TypesOfCoinExtensions
    {
        /// <summary>
        /// Gets the monetary value of the specified coin type.
        /// </summary>
        /// <param name="coinType">The coin type.</param>
        /// <returns>The monetary value of the coin as a decimal (e.g., £1 returns 1.00).</returns>
        public static decimal GetValue(this TypesOfCoin coinType)
        {
            return (decimal)coinType / 100;
        }

        /// <summary>
        /// Gets the display name of the specified coin type.
        /// </summary>
        /// <param name="coinType">The coin type.</param>
        /// <returns>A string representing the name of the coin (e.g., "£1", "50p").</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the coin type is invalid or not recognized.
        /// </exception>
        public static string GetName(this TypesOfCoin coinType)
        {
            return coinType switch
            {
                TypesOfCoin.OnePenny => "1p",
                TypesOfCoin.TwoPence => "2p",
                TypesOfCoin.FivePence => "5p",
                TypesOfCoin.TenPence => "10p",
                TypesOfCoin.TwentyPence => "20p",
                TypesOfCoin.FiftyPence => "50p",
                TypesOfCoin.OnePound => "£1",
                TypesOfCoin.TwoPound => "£2",
                _ => throw new ArgumentOutOfRangeException(nameof(coinType), "Invalid coin type.")
            };
        }
    }
}
