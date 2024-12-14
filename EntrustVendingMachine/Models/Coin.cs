using System;
using EntrustVendingMachine.Extensions;

namespace EntrustVendingMachine.Models
{
    public class Coin
    {
        public TypesOfCoin CoinType { get; set; }
        public int Quantity { get; set; }

        public Coin(TypesOfCoin coinType, int quantity)
        {
            CoinType = coinType;
            Quantity = quantity;
        }

        public string GetName()
        {
            return CoinType.GetName();
        }

        public decimal GetValue()
        {
            return CoinType.GetValue();
        }
    }
}
