using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAndBonds.Console.Models
{
    public class PurchasedSecurity
    {
        public Security Security { get; }
        public int Quantity { get; set; }

        public PurchasedSecurity(Security security)
        {
            Security = security;
            Quantity = 0;
        }
    }
}
