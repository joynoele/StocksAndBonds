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

        /// <summary>
        /// Initialize and refresh the game board to start a game.
        /// Does NOT setup new prices
        /// </summary>
        public void Initialize()
        {
            Year = 0;
            BoardMarket = MarketDirection.NotSet;
            BoardSecurities.Initialize();
        }

        /// <summary>
        /// Initialize and refresh the game board to start a game.
        /// Sets up new prices and sets the game ready for play at year 1
        /// </summary>
        public void Initialize(int marketRoll, int sum2D6)
        {
            Year = 0;
            BoardMarket = MarketDirection.NotSet;
            BoardSecurities.Initialize();
            AdvanceBoardYear(marketRoll, sum2D6);
        }

        /// <summary>
        /// Advance the board andother year and another roll
        /// </summary>
        /// <param name="marketRoll"></param>
        /// <param name="sum2D6"></param>
        public void AdvanceBoard(int marketRoll, int sum2D6)
        {
            if (marketRoll % 2 == 1) BoardMarket = MarketDirection.Bear;
            if (marketRoll % 2 == 0) BoardMarket = MarketDirection.Bull;
            BoardSecurities.AdjustPrice(BoardMarket, sum2D6);
        }

        public void AdvanceBoardYear(int marketRoll, int sum2D6)
        {
            Year++;
            AdvanceBoard(marketRoll, sum2D6);
        }

        public void PrintBoard()
        {
            BoardSecurities.PrintStatus();
        }
    }
}
