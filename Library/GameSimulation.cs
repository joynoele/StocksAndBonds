﻿using Library.Models;
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
        private readonly int _maxRounds;
        private Random _die;
        private readonly BoardSecurities _boardSecurities;
        private IList<IAiPlayer> _players;

        public GameSimulation(int rounds, BoardSecurities boardSecurities, Random die, IList<IAiPlayer> players)
        {
            _maxRounds = rounds;
            _boardSecurities = boardSecurities;
            _die = die;
            _players = players ?? new List<IAiPlayer>() { };
        }

        public void CaptureSecurityBehavior_Regression(string writeLocation, int simulationRuns)
        {
            Console.WriteLine("====== starting ======");
            StockBoard board = new StockBoard(_boardSecurities);
            using (StreamWriter writer = new StreamWriter(writeLocation))
            {
                writer.WriteLine("Run,Security,Year,Price,Delta,IsSplit,IsBust,Yield,RateOfReturn");
                for (int run = 1; run <= simulationRuns; run++)
                {
                    board.Initialize(RollD6(1), RollD6(2));
                    Console.Write($"\nRunning simulation {run} of {simulationRuns}");

                    while (board.Year <= _maxRounds)
                    {
                        Console.Write($"..{board.Year}");
                        foreach (var boardSecurity in board.BoardSecurities)
                        {
                            /*
                             * SplitMultiplier,s=1|2, accounts for where the stock has split
                             * Return,r = s*yield + costChange [e.g. purchased 1 unit for $100, sold for $110 + $2 yield = $12 return]
                             * Rate of Return,rrt = return/(s*cost - costChange) [e.g. above example would be $12/100 = 12%]
                             */
                            var s = boardSecurity.IsSplit ? 2 : 1;
                            decimal r = 0;
                            decimal rrt = 0;
                            try
                            {
                                if (boardSecurity.IsBust)
                                {
                                    // Because a busted security gets reset right away, we need to ignore the currently posted price/price changes
                                    r = -boardSecurity.CostChange;
                                    rrt = -1;
                                }
                                else
                                {
                                    r = (s * boardSecurity.CollectYieldAmt) + boardSecurity.CostChange;
                                    rrt = r / ((s * boardSecurity.CostPerShare) - boardSecurity.CostChange);
                                }
                            }
                            catch (DivideByZeroException)
                            {
                                rrt = 0;
                                Console.WriteLine($"Attempted to divide by zero ({boardSecurity.Name})\n");
                            }
                            writer.WriteLine($"{run},{boardSecurity.ShortName},{board.Year},{boardSecurity.CostPerShare},{boardSecurity.CostChange},{boardSecurity.IsSplit},{boardSecurity.IsBust},{boardSecurity.CollectYieldAmt},{rrt}");
                        }
                        board.AdvanceBoardYear(RollD6(1), RollD6(2));
                    }
                }
            }
            Console.WriteLine("====== completed ======");
        }

        public void PlayAiSimulation()
        {
            var board = new StockBoard(_boardSecurities);
            var tempRoll = RollD6(2);

            while (board.Year < _maxRounds)
            {
                Console.WriteLine($"\n======= YEAR {board.Year+1} of {_maxRounds} ======");
                AdvanceRound(board);
                Console.WriteLine($"--------------");

                // Take Turns
                foreach (var ai in _players)
                {
                    ai.TakeTurn(board.BoardSecurities.Assets, board.Year);
                }

                // Print round end summary
                Console.WriteLine();
                foreach (var player in _players)
                {
                    Console.Write($"({player.TotalWealth(board.BoardSecurities.Assets)}) ");
                    player.PrintStatus();
                }

                Console.ReadLine();
            } 

            // One last round to see how all investments paid off
            Console.WriteLine($"\n======= CLOSING ======");
            AdvanceRound(board);
            Console.WriteLine($"--------------");
            foreach (var player in _players)
            {
                player.CashOut(board.BoardSecurities.Assets);
            }

            Console.WriteLine($"======= GAME OVER ======");
            PrintPlayerWinners();
        }

        // Advances the board and the player statuses
        private void AdvanceRound(StockBoard board)
        {
            // Collect Yield (if any) 
            foreach (var ai in _players)
            {
                ai.AddYield(board.BoardSecurities.Assets);
            }

            // Change prices
            var tempRoll = RollD6(2);
            board.AdvanceBoardYear(RollD6(1), tempRoll);
            if (board.BoardMarket == MarketDirection.Bull)
                Console.WriteLine($"+++ Rolled {tempRoll} on {board.BoardMarket} market +++");
            else
                Console.WriteLine($"--- Rolled {tempRoll} on {board.BoardMarket} market ---");

            foreach (var asset in board.BoardSecurities)
            {
                // Award securities for players holding split stocks
                if (asset.IsSplit)
                {
                    Console.WriteLine($"{asset.ShortName} split!");
                    foreach (var player in _players) { player.SplitOwnedSecurity(asset); }
                }

                // Revoke securities for players holding sunk stocks
                if (asset.IsBust)
                {
                    Console.WriteLine($"{asset.ShortName} bust!");
                    foreach (var player in _players) { player.ForfitBustSecurity(asset); }
                }
            }
            board.PrintBoard();
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

        private void PrintPlayerWinners()
        {
            var winner = _players.OrderByDescending(p => p.Balance).First();
            Console.WriteLine($"{winner.Name} won with ${winner.Balance}");
            var losers = _players.OrderByDescending(p => p.Balance).Skip(1);
            foreach (var l in losers)
            {
                Console.WriteLine($"{l.Name}: ${l.Balance}");
            }
        }
    }
}
