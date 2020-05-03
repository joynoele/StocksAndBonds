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

        public void TakeTurn(IList<Asset> asset, int year)
        {
            if (Reinvest || year > 1)
                MaxBuy(asset.First(a => a.Id == IndexSecurity.Id));
        }

        public override void PrintStatus()
        {
            Console.WriteLine($"{Name}: ${Balance} {GetAsset(IndexSecurity).Quantity} shares");
        }

    }
}
