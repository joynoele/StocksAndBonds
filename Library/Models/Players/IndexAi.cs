using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class IndexAi : Player, IIndexAi
    {
        private static string strategy = "Keep it simple, go for the winner!";
        public Security IndexSecurity { get; private set; }
        public bool Reinvest { get; private set; }

        public IndexAi(int initialBalance, Security index, bool reinvest) : base("Index-" + index + (reinvest ? "-reinvest" : ""), initialBalance, strategy)
        {
            IndexSecurity = index;
            Reinvest = reinvest;
        }

        public void TakeTurn(IList<BoardSecurity> securities)
        {
            // Poor man's way of determining if this is year 1 or not
            if (Reinvest || (Portfolio.Sum(s => s.Quantity) == 0 && Balance != 0))
                MaxBuy(securities.First(s => s.Security == IndexSecurity));
        }

        public override void PrintStatus()
        {
            Console.WriteLine($"{Name}: ${Balance} {Portfolio.First(s => s.Security == IndexSecurity).Quantity} shares");
        }
    }
}
