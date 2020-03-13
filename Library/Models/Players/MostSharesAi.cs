using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class MostSharesAi : Player, IAiPlayer
    {
        public MostSharesAi(int initialBalance) : base("MostSharesDilbert", initialBalance)
        {
        }

        public void TakeTurn(IList<BoardSecurity> securities)
        {
            var securitiesByCost = CheapestSecurities(securities);
            bool didBuy = false;

            // Can I buy anything with current balance?
            foreach (var security in securitiesByCost)
            {
                if (MaxBuy(security) > 0)
                    didBuy = true;
            }
            if (didBuy) return;

            // If I sell everything, can I get more shares of something?
            var currentShareQuantity = Portfolio.Sum(s => s.Quantity);
            var cheapestSecurity = securitiesByCost.First();
            var costPerBundle = cheapestSecurity.CostPerShare * 10;
            var bundlesCanAfford = Balance / costPerBundle;

            if (bundlesCanAfford*10 > currentShareQuantity)
            {
                SellAll(securities);
                MaxBuy(securitiesByCost.First());
            }
        }

        private List<BoardSecurity> CheapestSecurities(IList<BoardSecurity> boardSecurities)
        {
            return boardSecurities.OrderBy(s1 => s1.CostPerShare).Where(s2 => s2.CostPerShare > 0).ToList();
        }
    }
}
