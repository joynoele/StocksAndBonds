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
            SecurityFactory.LoadSecurities("C:\\Repo\\StocksAndBonds\\securities.json");
            var players = InitializePlayers();
            StockBoard GameBoard = new StockBoard(SecurityFactory.BoardSecurities);

            System.Console.WriteLine("===== Starting simulation of 'Stocks & Bonds' =====");
            foreach (var player in players)
            {
                System.Console.WriteLine($"Welcome {player.Name}!");
            }
            System.Console.WriteLine("Ready???");
            System.Console.ReadLine();

            GameSimulation game = new GameSimulation(MaxYears, GameBoard, new Random());
            game.PlayAiSimulation(players);

            System.Console.ReadLine();
        }

        private static IList<IAiPlayer> InitializePlayers()
        {
            var player1 = new MostSharesAi(DefaultStartingBalance);
            var player2 = new MostChangedAi(DefaultStartingBalance);
            var player3 = new YieldAi(DefaultStartingBalance);
            return new List<IAiPlayer>() { player1, player2, player3 };
        }
    }
}
