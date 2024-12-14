using System;
using EntrustVendingMachine.Models;

namespace EntrustVendingMachine.Extensions
{
    public static class TypesOfCoinExtensions
    {
        public static decimal GetValue(this TypesOfCoin coinType)
        {
            return (decimal)coinType / 100;
        }

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
