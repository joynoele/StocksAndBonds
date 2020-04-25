using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class MostSharesAi : Player, IAiPlayer
    {
        private static string strategy = "Hostess with the mostess";
        public MostSharesAi(int initialBalance) : base("MostSharesDilbert", initialBalance, strategy)
        {
        }

        public void Observe(IList<Asset> securities, int year)
        {
            // do nothing
        }

        public void TakeTurn(IList<Asset> securities, int year)
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

        private List<Asset> CheapestSecurities(IList<Asset> boardSecurities)
        {
            // Anything under 30 (magic number) has a tendancy to go down to 0. Don't buy.
            return boardSecurities.Where(s2 => s2.CostPerShare > 30).OrderBy(s1 => s1.CostPerShare).ToList();
        }
    }
}
