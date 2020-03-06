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
        private int _marketRoll;
        private bool _isBear => _marketRoll % 2 == 1;
        private bool _isBull => _marketRoll % 2 == 0; // added for completeness
        private Random _die;
        private Board _board;

        public GameSimulation(int years, Board board, Random die)
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
                System.Console.WriteLine($"\n======= YEAR {year} of {_maxYears} ======");
                _marketRoll = RollD6();
                var d6 = RollD6(2);
                System.Console.WriteLine($"Rolled {d6} on " + (_isBear ? " BEAR marketRoll" : "BULL marketRoll"));
                _board.SetupNextYear(_isBear, d6);
                _board.PrintBoard();
                System.Console.WriteLine($"--------------");

                foreach (var ai in players)
                {
                    ai.TakeTurn(_board);
                }

                foreach (var ai in players)
                {
                    ai.AddYield();
                }

                System.Console.WriteLine();
                foreach (var player in players)
                {
                    player.PrintStatus();
                }

                System.Console.ReadLine();
            }
            foreach (var player in players)
            {
                player.CashOut(_board);
            }

            System.Console.WriteLine($"======= GAME OVER ======");
            PrintPlayerWinners(players);
        }

        private int RollD6(int quantity = 1)
        {
            var rollSum = 0;
            for (int i = 0; i <= quantity; i++)
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
                System.Console.WriteLine($"{l.Name}: ${l.Balance}");
            }
        }
    }
}
