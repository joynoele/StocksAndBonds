using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library.Models.Players
{
    public class MaximizeRrtAi : GrowthAi, IGrowthAi
    {
        
        public MaximizeRrtAi(int initialBalance, int maxYears, IEnumerable<Asset> assetsInPlay) 
            : base(initialBalance, "MaxReturn", null, maxYears, assetsInPlay)
        {
        }

        public void TakeTurn(IList<Asset> assets, int currentYear)
        {
            UpdateLedger(assets, currentYear);

            // evaluate the securities based on calculations in the ledger
            var purchaseOrder = AnalysisTables
                .Where(w => w.Value.AvgRateOfReturn[currentYear - 1] > 0)
                .OrderByDescending(x => x.Value.AvgRateOfReturn[currentYear - 1]);

            // TODO: buy/sell
            // TODO: call the cashout or sell all instead once access modifiers are figured out
            foreach (var b in assets)
            {
                Sell(b);
            }
            Console.WriteLine($"{this.Name}'s top 3 picks: " +
                $"{purchaseOrder.First().Key} ({purchaseOrder.First().Value.AvgRateOfReturn[currentYear-1]:p2}), " +
                $"{purchaseOrder.Skip(1).Take(1).First().Key} ({purchaseOrder.Skip(1).Take(1).First().Value.AvgRateOfReturn[currentYear-1]:p2}), " +
                $"{purchaseOrder.Skip(2).Take(1).First().Key} ({purchaseOrder.Skip(2).Take(1).First().Value.AvgRateOfReturn[currentYear-1]:p2})");
            foreach (var puchase in purchaseOrder)
            {
                MaxBuy(puchase.Key);
            }
        }
    }
}
