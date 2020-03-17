using Library.Models;
using Library.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library
{
    public class GameSimulation
    {
        private readonly int _maxYears;
        private Random _die;
        private StockBoard _board;

        public GameSimulation(int years, StockBoard board, Random die)
        {
            _maxYears = years;
            _board = board;
            _die = die;
        }

        public void PlayAiSimulation(IList<IAiPlayer> players)
        {
            int year = 1;
            for (; year <= _maxYears; year++)
            {
                Console.WriteLine($"\n======= YEAR {year} of {_maxYears} ======");
                // Collect Yield (if any) - 
                // TODO: check if this happen at the beginning or the end of the turn
                foreach (var ai in players)
                {
                    ai.AddYield(_board.BoardSecurities);
                }

                // Setup board for next year
                var tempRoll = RollD6(2);
                _board.AdvanceYear(RollD6(1), tempRoll);
                Console.WriteLine($"Rolled {tempRoll} on {_board.BoardMarket} market");
                foreach (var boardSecurity in _board.BoardSecurities)
                {
                    if (boardSecurity.IsSplit)
                    {
                        Console.WriteLine($"{boardSecurity.Security} split!");
                        foreach (var player in players) { player.SplitOwnedSecurity(boardSecurity.Security); }
                    }
                    if (boardSecurity.CostPerShare <= 0)
                    {
                        Console.WriteLine($"{boardSecurity.Security} sunk!");
                        foreach (var player in players) { player.ForfitSunkSecurity(boardSecurity.Security); }
                    }
                }
                _board.PrintBoard();
                Console.WriteLine($"--------------");

                // Take Turns
                foreach (var ai in players)
                {
                    ai.TakeTurn(_board.BoardSecurities);
                }

                // Print round end summary
                Console.WriteLine();
                foreach (var player in players)
                {
                    Console.Write($"({player.TotalWealth(_board.BoardSecurities)}) ");
                    player.PrintStatus();
                }

                Console.ReadLine();
            }
            foreach (var player in players)
            {
                player.CashOut(_board.BoardSecurities);
            }

            Console.WriteLine($"======= GAME OVER ======");
            PrintPlayerWinners(players);
        }

        private int RollD6(int quantity = 1)
        {
            var rollSum = 0;
            for (int i = 0; i < quantity; i++)
            {
                rollSum += _die.Next(1, 6);
            }

            return rollSum;
        }

        private void PrintPlayerWinners(IList<IAiPlayer> players)
        {
            var winner = players.OrderByDescending(p => p.Balance).First();
            Console.WriteLine($"{winner.Name} won with ${winner.Balance}");
            var losers = players.OrderByDescending(p => p.Balance).Skip(1);
            foreach (var l in losers)
            {
                Console.WriteLine($"{l.Name}: ${l.Balance}");
            }
        }
    }
}
