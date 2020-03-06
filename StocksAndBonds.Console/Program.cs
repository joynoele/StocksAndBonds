using Library;
using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;

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
            Board GameBoard = new Board(SecurityFactory.BoardSecurities);

            System.Console.WriteLine("===== Starting simulation of 'Stocks & Bonds' =====");
            foreach (var player in players)
            {
                System.Console.WriteLine($"Welcome {player.Name}!");
            }
            System.Console.WriteLine("Ready???\n");
            System.Console.ReadLine();

            GameSimulation game = new GameSimulation(MaxYears, players, GameBoard, new Random());
            game.PlayGame();

            System.Console.ReadLine();
        }

        private static IList<Player> InitializePlayers()
        {
            var player1 = new Player("LowDilbert", DefaultStartingBalance);
            var player2 = new Player("GrowthDilbert", DefaultStartingBalance);
            var player3 = new Player("YieldDilbert", DefaultStartingBalance);
            return new List<Player>(){player1, player2, player3};
        }
    }
}
