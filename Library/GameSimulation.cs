using Library.Models;
using Library.Models.Players;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void CaptureSecurityBehavior(string writeLocation, int simulationRuns)
        {
            Console.WriteLine("====== starting ======");
            var run = 0;
            using (StreamWriter writer = new StreamWriter(writeLocation))
            {
                writer.WriteLine("security,year,price,delta,isSplit,yield,return,rateofreturn");
                while (run < simulationRuns)
                {
                    run++;
                    Console.WriteLine($"Running simulation {run} of {simulationRuns}");
                    for (int year = 1; year <= _maxYears; year++)
                    {
                        var tempRoll = RollD6(2);
                        _board.AdvanceYear(RollD6(1), tempRoll);
                        foreach (var boardSecurity in _board.BoardSecurities)
                        {
                            /*
                             * SplitMultiplier,s=1|2, accounts for where the stock has split
                             * Return,r = s*yield + costChange [e.g. purchased 1 unit for $100, sold for $110 + $2 yield = $12 return]
                             * Rate of Return,rrt = return/(s*cost - costChange) [e.g. above example would be $12/100 = 12%]
                             */
                            var s = boardSecurity.IsSplit ? 2 : 1;
                            double r = 0; 
                            double rrt = 0;
                            try
                            {
                                r = (s * boardSecurity.Security.YieldPer10Shares) + boardSecurity.CostChange;
                                rrt = r / ((s * boardSecurity.CostPerShare) - boardSecurity.CostChange);
                            }
                            catch(DivideByZeroException) {
                                rrt = 0;
                                Console.WriteLine($"Attempted to divide by zero ({boardSecurity.Security.Name})");
                            } finally
                            {
                                if (Double.IsInfinity(rrt))
                                {
                                    rrt = 0;
                                    Console.WriteLine($"rrt was infinite ({boardSecurity.Security.Name})");
                                }
                            }
                            writer.WriteLine($"{boardSecurity.Security.ShortName},{year},{boardSecurity.CostPerShare},{boardSecurity.CostChange},{boardSecurity.IsSplit},{boardSecurity.Security.YieldPer10Shares},{r},{rrt}");
                        }
                    }
                } 
            }
            Console.Write("====== completed ======");
        }

        public void PlayAiSimulation(IList<IAiPlayer> players)
        {
            for (int year = 1; year <= _maxYears; year++)
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
