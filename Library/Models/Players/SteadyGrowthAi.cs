using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using StocksAndBondsML.Model;

namespace Library.Models.Players
{
    public class SteadyGrowthAi : Player, IAiPlayer
    {
        private static string strategy = "Slow and steady (growth) wins the race";

        public SteadyGrowthAi(int initialBalance) : base("GrowthDilbert", initialBalance, strategy)
        {
        }

        public void TakeTurn(IList<BoardSecurity> securities, int year)
        {

            var costChanges = securities.Select(s => s.CostChange);

            /* Sell off everything that wasn't "steady" and buy top picks within the steady growth */
            // Calculation thanks to @JohnathanDeMarks: https://stackoverflow.com/questions/3141692/standard-deviation-of-generic-list
            //double avgGrowth = costChanges.Average();
            //double stdDev = Math.Sqrt(costChanges.Average(v => Math.Pow(v - avgGrowth, 2)));
            //List<BoardSecurity> securitiesByGrowth = SteadyGrowth(securities, avgGrowth, stdDev);

            //var nonGrowth = securities.Except(securitiesByGrowth).ToList();
            //SellAll(nonGrowth);

            CashOut(securities);
            var securitiesByGrowth = TopGrowthPics(securities, year);
            

            foreach (var goodBuy in securitiesByGrowth)
            {
                MaxBuy(goodBuy);
            }
        }

        private IEnumerable<BoardSecurity> TopGrowthPics(IList<BoardSecurity> boardSecurities, int year)
        {
            /**
            Use ML.NET to see what are the best deals out there:
            using StocksAndBondsML.Model;
            // Add input data
            var input = new ModelInput();
            // Load model and predict output of sample data
            ModelOutput result = ConsumeModel.Predict(input);
            **/
            var withRrt = new Dictionary<BoardSecurity, float>();

            foreach (BoardSecurity s in boardSecurities)
            {
                var input = new ModelInput() { Security = s.Security.ShortName, Year = year, Price = s.CostPerShare, IsSplit = s.IsSplit};
                ModelOutput result = ConsumeModel.Predict(input);
                Console.WriteLine($"{s.Security.ShortName} has a predicted rrt of {result.Score:P2}: year={year}, price={s.CostPerShare}, IsSplit={s.IsSplit}");
                if (result.Score > 0) withRrt.Add(s, result.Score);
            }
            var topPicks = withRrt.OrderByDescending(x => x.Value).Select(y => y.Key);
            Console.WriteLine($"Growth Securities to buy (in order) are: {string.Join(", ", topPicks.Select(x => x.Security.ShortName)) }");
            return topPicks;
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
