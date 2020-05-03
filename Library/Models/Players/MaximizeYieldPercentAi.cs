using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Library.Models.Players
{
    public class MaximizeYieldPercentAi : GrowthAi, IGrowthAi
    {
        
        public MaximizeYieldPercentAi(int initialBalance, int maxYears, IEnumerable<Asset> assetsInPlay) 
            : base(initialBalance, "MaxYieldPercent", null, maxYears, assetsInPlay)
        {
        }

        public void TakeTurn(IList<Asset> assets, int year)
        {
            UpdateLedger(assets, year);

            // evaluate the securities based on calculations in the ledger
            var purchaseOrder = AnalysisTables
                .OrderByDescending(x => x.Value.YieldPercent[year-1])
                .Select(y => y.Key);
            Console.WriteLine($"{this.Name}'s top 3 picks: {purchaseOrder.First()}, {purchaseOrder.Skip(1).Take(1)}, {purchaseOrder.Skip(2).Take(1)}");
            
            // TODO: buy/sell
            foreach (var puchase in purchaseOrder)
            {
                MaxBuy(puchase);
            }
        }

    }
}
