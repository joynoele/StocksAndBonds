using StocksAndBonds.Console.Models;
using System.Collections.Generic;

namespace StocksAndBonds.Console
{
    public static class SecurityFactory
    {
        private static IList<Security> _baseSecurities = new List<Security>()
            {
                new Security(0, "C City Bonds"),
                new Security(5, "Stryker Drilling"),
                new Security(9, "Tri-City Transit", 1)}
            ;

        public static IList<BoardSecurity> InitializeBoardSecurities()
        {
            BoardSecurity CCityBonds = new BoardSecurity(_baseSecurities[0],
                new Dictionary<int, int>
                {
                    {2, -1}, {3, -1}, {4, -1}, {5, -1}, {6, -1},{ 7, -1}, {8, -1}, {9, -1}, {10, -1}, {11, -1}, {12, -1}
                },
                new Dictionary<int, int>
                {
                    {2, 1}, {3, 1}, {4, 1},{5, 2}, {6, 2},{ 7, 2}, {8, 3}, {9, 3}, {10, 3}, {11, 4}, {12, 4}
                });

            BoardSecurity StrykerDrilling = new BoardSecurity(_baseSecurities[1],
                new Dictionary<int, int>
                {
                    {2, -2}, {3, -3}, {4, -4}, {5, -5}, {6, -6}, {7, -7},{8, -8}, {9, -9}, {10, -10}, {11, -11}, {12, -12}
                },
                new Dictionary<int, int>
                {
                    {2, 3}, {3, 4}, {4, 5}, {5, 6}, {6, 7}, {7, 8},{8, 9}, {9, 10}, {10, 11}, {11, 12}, {12, 13}
                });

            BoardSecurity TriCityTransit = new BoardSecurity(_baseSecurities[2],
                new Dictionary<int, int>
                {
                    {2, -1}, {3, -1}, {4, -1}, {5, -2}, {6, -2}, {7, -2},{8, -3}, {9, -3}, {10, -3}, {11, -3}, {12, -3}
                },
                new Dictionary<int, int>
                {
                    {2, 2}, {3, 3}, {4, 3}, {5, 4}, {6, 4}, {7, 5},{8, 5}, {9, 5}, {10, 6}, {11, 6}, {12, 6}
                });

            // TODO: add other securities here
            //.....

            return new List<BoardSecurity>() { CCityBonds, StrykerDrilling, TriCityTransit };
        }

        public static IList<PurchasedSecurity> InitializePlayerPortfolio()
        {
            var emptyPortfolio = new List<PurchasedSecurity>();
            foreach (var sec in _baseSecurities)
            {
                emptyPortfolio.Add(new PurchasedSecurity(sec));
            }
            return emptyPortfolio;
        }
    }
}
