using Library.Models;
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
        private IList<Player> _players;
        private Board _board;

        public GameSimulation(int years, IList<Player> players, Board board, Random die)
        {
            _maxYears = years;
            _players = players;
            _board = board;
            _die = die;
        }

        public void PlayGame()
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
                /**
                 * Time for players to play
                 * Fill out with some AI later.
                 */
                // Player1
                _players[0].SellAll(_board.BoardSecurities);
                var lowestCostSecurity = _board.BoardSecurities.OrderBy(s => s.CostPerShare).First();
                _players[0].MaxBuy(lowestCostSecurity);
                _players[0].AddYield();
                // Player2
                _players[1].SellAll(_board.BoardSecurities);
                var highestGrowthSecurity = _board.BoardSecurities.OrderByDescending(s => s.CostChange).First();
                _players[1].MaxBuy(highestGrowthSecurity);
                _players[1].AddYield();
                // Player2
                _players[2].SellAll(_board.BoardSecurities);
                var mostYield = _board.BoardSecurities.OrderByDescending(s => s.Security.YieldPer10Shares).First();
                _players[2].MaxBuy(mostYield);
                _players[2].AddYield();
                /*****/

                System.Console.WriteLine();
                foreach (var player in _players)
                {
                    player.PrintStatus();
                }

                System.Console.ReadLine();
            }
            foreach (var player in _players)
            {
                player.SellAll(_board.BoardSecurities);
            }

            System.Console.WriteLine($"======= GAME OVER ======");
            PrintPlayerWinners();
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

        private void PrintPlayerWinners()
        {
            var winner = _players.OrderByDescending(p => p.Balance).First();
            System.Console.WriteLine($"{winner.Name} won with ${winner.Balance}");
            var losers = _players.OrderByDescending(p => p.Balance).Skip(1);
            foreach (var l in losers)
            {
                System.Console.WriteLine($"{l.Name}: ${l.Balance}");
            }
        }
    }
}
