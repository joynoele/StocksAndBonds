using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public class YieldAi : Player, IAiPlayer
    {
        public YieldAi(int initialBalance) : base("YieldDilbert", initialBalance)
        {
        }

        public void TakeTurn(Board board)
        {
            // sell everything
            SellAll(board.BoardSecurities);
            // buy the lowest cost thing
            MaxBuy(MostYield(board.BoardSecurities));
        }

        private BoardSecurity MostYield(IList<BoardSecurity> boardSecurities)
        {
            return boardSecurities.OrderByDescending(s => s.Security.YieldPer10Shares).First();
        }
    }
}
