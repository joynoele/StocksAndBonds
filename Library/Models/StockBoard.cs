using System;
using System.Collections.Generic;

namespace Library.Models
{
    public class StockBoard
    {
        public BoardSecurities BoardSecurities { get; private set; }
        public int Year { get; private set; }
        public MarketDirection BoardMarket { get; private set; }

        public StockBoard(BoardSecurities boardSecurities)
        {
            BoardSecurities = boardSecurities;
            Initialize();
        }

        // Setup gameboard to start a fresh game
        public void Initialize()
        {
            Year = 0;
            BoardMarket = MarketDirection.NotSet;
            BoardSecurities.Initialize();
        }

        public void AdvanceYear(int marketRoll, int sum2D6)
        {
            if (marketRoll % 2 == 1) BoardMarket = MarketDirection.Bear;
            if (marketRoll % 2 == 0) BoardMarket = MarketDirection.Bull;
            Year++;
            BoardSecurities.AdjustPrice(BoardMarket, sum2D6);
        }

        public void PrintBoard()
        {
            BoardSecurities.PrintStatus();
        }
    }
}
