using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;
using StocksAndBonds.Console.Models;

namespace StocksAndBonds.Console
{
    public class Program
    {
        private static readonly int DefaultStartingBalance = 5000;
        private static int MaxYears = 10;
        private static int marketRoll;
        public static bool IsBear => marketRoll % 2 == 1;
        public static bool IsBull => marketRoll % 2 == 0;
        private static Random die;

        public static void Main(string[] args)
        {
            System.Console.WriteLine("===== Starting simulation of 'Stocks & Bonds' =====");
            var securities = SecurityFactory.InitializeBoardSecurities();

            var players = InitializePlayers();
            foreach (var player in players)
            {
                System.Console.WriteLine($"Welcome {player.Name}!");
            }

            Board GameBoard = new Board(securities);
            die = new Random();

            System.Console.WriteLine("Ready???\n");
            System.Console.ReadLine();
            
            PlayGame(GameBoard, players);
            System.Console.ReadLine();
        }

        private static IList<Player> InitializePlayers()
        {
            var player1 = new Player("LowDilbert", DefaultStartingBalance);
            var player2 = new Player("GrowthDilbert", DefaultStartingBalance);
            var player3 = new Player("YieldDilbert", DefaultStartingBalance);
            return new List<Player>(){player1, player2, player3};
        }

        private static int RollD6(int quantity = 1)
        {
            var rollSum = 0;
            for (int i = 0; i <= quantity; i++)
            {
                rollSum += die.Next(1, 6);
            }

            return rollSum;
        }

        private static void PlayGame(Board board, IList<Player> players)
        {
            int year = 1;
            for(; year <= MaxYears; year++)
            {
                System.Console.WriteLine($"\n======= YEAR {year} of {MaxYears} ======");
                marketRoll = RollD6();
                var d6 = RollD6(2);
                System.Console.WriteLine($"Rolled {d6} on " + (IsBear ? " BEAR marketRoll" : "BULL marketRoll"));
                board.SetupNextYear(IsBear, d6);
                board.PrintBoard();
                System.Console.WriteLine($"--------------");
                /**
                 * Time for players to play
                 * Fill out with some AI later.
                 */
                // Player1
                players[0].SellAll(board.BoardSecurities);
                var lowestCostSecurity = board.BoardSecurities.OrderBy(s => s.CostPerShare).First();
                players[0].MaxBuy(lowestCostSecurity);
                players[0].AddYield();
                // Player2
                players[1].SellAll(board.BoardSecurities);
                var highestGrowthSecurity = board.BoardSecurities.OrderByDescending(s => s.CostChange).First();
                players[1].MaxBuy(highestGrowthSecurity);
                players[1].AddYield();
                // Player2
                players[2].SellAll(board.BoardSecurities);
                var mostYield = board.BoardSecurities.OrderByDescending(s => s.Security.Yield).First();
                players[2].MaxBuy(mostYield);
                players[2].AddYield();
                /*****/

                System.Console.WriteLine();
                foreach (var player in players)
                {
                    player.PrintStatus();
                }

                System.Console.ReadLine();
            }
            foreach (var player in players)
            {
                player.SellAll(board.BoardSecurities);
            }

            System.Console.WriteLine($"======= GAME OVER ======");
            PrintPlayerWinners(players);
        }

        private static void PrintPlayerWinners(IList<Player> players)
        {
            var winner = players.OrderByDescending(p => p.Balance).First();
            System.Console.WriteLine($"{winner.Name} won with ${winner.Balance}");
            var losers = players.OrderByDescending(p => p.Balance).Skip(1);
            foreach (var l in losers)
            {
                System.Console.WriteLine($"{l.Name}: ${l.Balance}");
            }
        }

    }
}
