using EntrustVendingMachine.Enums;
using EntrustVendingMachine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
