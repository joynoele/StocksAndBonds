using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class SteadyGrowthAi : Player, IAiPlayer
    {
        private static string strategy = "Slow and steady (growth) wins the race";

        public SteadyGrowthAi(int initialBalance) : base("GrowthDilbert", initialBalance, strategy)
        {
        }

        public void TakeTurn(IList<BoardSecurity> securities)
        {
            var costChanges = securities.Select(s => s.CostChange);

            /* Sell off everything that wasn't "steady" and buy top picks within the steady growth */
            // Calculation thanks to @JohnathanDeMarks: https://stackoverflow.com/questions/3141692/standard-deviation-of-generic-list
            double avgGrowth = costChanges.Average();
            double stdDev = Math.Sqrt(costChanges.Average(v => Math.Pow(v - avgGrowth, 2)));
            List<BoardSecurity> securitiesByGrowth = SteadyGrowth(securities, avgGrowth, stdDev);
            
            var nonGrowth = securities.Except(securitiesByGrowth).ToList();
            SellAll(nonGrowth);

            foreach (var goodBuy in securitiesByGrowth)
            {
                MaxBuy(goodBuy);
            }
        }

        private List<BoardSecurity> SteadyGrowth(IList<BoardSecurity> boardSecurities, double avgGrowth, double stdDev)
        {
            // Go after securities with the highest growth, within 1 standard deviation of all securities
            // Persue growth, but only within what is considered "normal" with respect to all securities
            var buyThese = boardSecurities
                .Where(s1 => s1.CostPerShare > 0 && s1.CostChange < avgGrowth + stdDev && s1.CostChange > avgGrowth - stdDev)
                .OrderByDescending(s2 => s2.CostChange)
                .ThenBy(s3 => s3.CostPerShare)
                .ToList();
            return buyThese;
        }
    }
}
