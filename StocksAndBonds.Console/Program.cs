using Library;
using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;
using Library.Models.Players;

namespace StocksAndBonds.Console
{
    public class Program
    {
        private static readonly int DefaultStartingBalance = 5000;
        private static int MaxYears = 10;

        public static void Main(string[] args)
        {
            SecurityFactory.LoadSecurities("..\\..\\..\\..\\securities.json");
            var players = InitializePlayers();
            StockBoard GameBoard = new StockBoard(SecurityFactory.BoardSecurities);

            System.Console.WriteLine("===== Starting simulation of 'Stocks & Bonds' =====");
            foreach (var player in players)
            {
                System.Console.WriteLine($"Welcome {player.Name}! `{player.Strategy}`");
            }
            System.Console.Write("Ready???");
            System.Console.ReadLine();

            GameSimulation game = new GameSimulation(MaxYears, GameBoard, new Random());
            game.PlayAiSimulation(players);

            System.Console.ReadLine();
        }

        private static IList<IAiPlayer> InitializePlayers()
        {
            var player1 = new MostSharesAi(DefaultStartingBalance);
            var player2 = new SteadyGrowthAi(DefaultStartingBalance);
            var player3 = new YieldAi(DefaultStartingBalance);
            var players = new List<IAiPlayer>() { player1, player2, player3 };

            foreach (var index in SecurityFactory.BoardSecurities)
            {
                players.Add(new IndexAi(DefaultStartingBalance, index.Security, false));
                if (index.Security.YieldPer10Shares > 0)
                    players.Add(new IndexAi(DefaultStartingBalance, index.Security, true));
            }

            return players;
        }
    }
}
