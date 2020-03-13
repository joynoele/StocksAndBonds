using System;
using System.Collections.Generic;

namespace Library.Models
{
    public class StockBoard
    {
        public IList<BoardSecurity> BoardSecurities { get; private set; }
        public int Year { get; private set; }
        public MarketDirection BoardMarket { get; private set; }

        public StockBoard(IList<BoardSecurity> boardSecurities)
        {
            BoardSecurities = boardSecurities;
            Year = 0;
        }

        public void AdvanceYear(int marketRoll, int sum2D6)
        {
            if (marketRoll % 2 == 1) BoardMarket = MarketDirection.Bear;
            if (marketRoll % 2 == 0) BoardMarket = MarketDirection.Bull;
            Year++;

            foreach (var b in BoardSecurities)
            {
                b.AdjustPrice(BoardMarket, sum2D6);
            }
        }

        public void PrintBoard()
        {
            foreach (var s in BoardSecurities)
            {
                var costChange = s.CostChange > 0 ? $"+{s.CostChange}" : $"{s.CostChange}";
                var costPerShare = s.IsSplit ? $"{s.CostPerShare}/{s.CostPerShare * 2}" : $"{ s.CostPerShare}";
                // padding is used to make the names line up
                Console.WriteLine($"{s.Security.Name.PadRight(30)}\t({costChange})\t{costPerShare}");
            }
        }
    }
}
