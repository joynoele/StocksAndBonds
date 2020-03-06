using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class LowCostAi : Player, IAiPlayer
    {
        public LowCostAi(int initialBalance) : base("LowDilbert", initialBalance)
        {
        }

        public void TakeTurn(Board board)
        {
            // sell everything
            SellAll(board.BoardSecurities);
            // buy the lowest cost thing
            MaxBuy(LowestSecurity(board.BoardSecurities));
        }

        private BoardSecurity LowestSecurity(IList<BoardSecurity> boardSecurities)
        {
            return boardSecurities.OrderBy(s => s.CostPerShare).First();
        }
    }
}
