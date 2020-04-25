using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class YieldAi : Player, IAiPlayer
    {
        private static string strategy = "Money make mo' money";
        public YieldAi(int initialBalance) : base("YieldDilbert", initialBalance, strategy)
        {
        }

        public void TakeTurn(IList<Asset> securities, int year)
        {
            var securitiesByYield = MostYield(securities);
            bool didBuy = false;

            // Can I buy anything with current balance?
            foreach (var asset in securitiesByYield)
            {
                if (MaxBuy(asset) > 0)
                    didBuy = true;
            }
            if (didBuy) return;

            // If I sell everything, can I get more shares of the next best yield?
            foreach (var asset in securitiesByYield)
            {
                var currentShareQuantity = Portfolio.First(p => p.Security.Id == asset.Id).Quantity;
                var costPerBundle = asset.CostPerShare * 10;
                var bundlesCanAfford = Balance / costPerBundle;

                if (bundlesCanAfford > 10 && bundlesCanAfford * 10 > currentShareQuantity)
                {
                    SellAll(securities);
                    MaxBuy(asset);
                }
            }
        }

        public void Observe(IList<Asset> securities, int year)
        {
            // do nothing
        }

        private IList<Asset> MostYield(IList<Asset> boardSecurities)
        {
            // Only look at securities that haven't fallen below the threshold where yield cannot be collected
            return boardSecurities.Where(s2 => s2.CostPerShare > 30).OrderByDescending(s => s.CollectYieldAmt).ToList();
        }
    }
}
