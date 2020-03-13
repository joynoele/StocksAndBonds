using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class MostChangedAi : Player, IAiPlayer
    {
        public MostChangedAi(int initialBalance) : base("ChangeDilbert", initialBalance)
        {
        }

        public void TakeTurn(IList<BoardSecurity> securities)
        {
            var securitiesByGrowth = BiggestIncrease(securities);
            bool didBuy = false;

            // Can I buy anything with current balance?
            foreach (var security in securitiesByGrowth)
            {
                if (MaxBuy(security) > 0)
                    didBuy = true;
            }
            if (didBuy) return;

            // If I sell everything, can I get more shares of the next best growth?
            foreach (var security in securitiesByGrowth)
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

        private List<BoardSecurity> BiggestIncrease(IList<BoardSecurity> boardSecurities)
        {
            return boardSecurities.OrderByDescending(s => s.CostChange).Where(s2 => s2.CostPerShare > 0).ToList();
        }
    }
}
