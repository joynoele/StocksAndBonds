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

        public void TakeTurn(IList<BoardSecurity> securities)
        {
            var securitiesByYield = MostYield(securities);
            bool didBuy = false;

            // Can I buy anything with current balance?
            foreach (var security in securitiesByYield)
            {
                if (MaxBuy(security) > 0)
                    didBuy = true;
            }
            if (didBuy) return;

            // If I sell everything, can I get more shares of the next best yield?
            foreach (var security in securitiesByYield)
            {
                var currentShareQuantity = Portfolio.First(p => p.Security == security.Security).Quantity;
                var costPerBundle = security.CostPerShare * 10;
                var bundlesCanAfford = Balance / costPerBundle;

                if (bundlesCanAfford > 10 && bundlesCanAfford * 10 > currentShareQuantity)
                {
                    SellAll(securities);
                    MaxBuy(security);
                }
            }
        }

        private IList<BoardSecurity> MostYield(IList<BoardSecurity> boardSecurities)
        {
            // Only look at securities that haven't fallen below the threshold where yield cannot be collected
            return boardSecurities.Where(s2 => s2.CostPerShare > 30).OrderByDescending(s => s.Security.YieldPer10Shares).ToList();
        }
    }
}
