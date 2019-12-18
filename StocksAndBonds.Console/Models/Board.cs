using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAndBonds.Console.Models
{
    public class Board
    {
        public IList<BoardSecurity> BoardSecurities { get; private set; }

        public Board(IList<BoardSecurity> boardSecurities)
        {
            BoardSecurities = boardSecurities;
        }

        public void SetupNextYear(bool isBear, int dieRoll)
        {
            foreach (var b in BoardSecurities)
            {
                b.AdjustPrice(isBear, dieRoll);
            }
        }

        public void PrintBoard()
        {
            foreach (var s in BoardSecurities)
            {
                var costChange = s.CostChange > 0 ? $"+{s.CostChange}" : $"{s.CostChange}";
                System.Console.WriteLine(s.IsSplit
                    ? $"{s.Security.Name}\t({costChange})\t{s.CostPerShare}/{s.CostPerShare * 2} "
                    : $"{s.Security.Name}\t({costChange})\t{s.CostPerShare}");
            }
        }
    }
}
