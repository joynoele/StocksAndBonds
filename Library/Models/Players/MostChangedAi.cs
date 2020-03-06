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

        public void TakeTurn(Board board)
        {
            // sell everything
            SellAll(board.BoardSecurities);
            // buy the thing that changed the most (in the positive direction)
            MaxBuy(MostChanged(board.BoardSecurities));
        }

        private BoardSecurity MostChanged(IList<BoardSecurity> boardSecurities)
        {
            return boardSecurities.OrderByDescending(s => s.CostChange).First();
        }
    }
}
