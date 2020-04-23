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
        private readonly int _maxRounds;
        private Random _die;
        private readonly IList<BoardSecurity> _securities;
        private IList<IAiPlayer> _players;

        public GameSimulation(int rounds, IList<BoardSecurity> securities, Random die, IList<IAiPlayer> players)
        {
            _maxRounds = rounds;
            _securities = securities;
            _die = die;
            _players = players ?? new List<IAiPlayer>(){};
        }

        public void CaptureSecurityBehavior(string writeLocation, int simulationRuns)
        {
            Console.WriteLine("====== starting ======");
            StockBoard board = new StockBoard(_securities);
            using (StreamWriter writer = new StreamWriter(writeLocation))
            {
                writer.WriteLine("Run,Security,Year,Price,Delta,IsSplit,IsBust,Yield,ReturnOnInvestment,RateOfReturn");
                for (int run = 1; run <= simulationRuns; run++)
                {
                    board.Initialize();
                    //var foo0 = board.BoardSecurities.First(st => st.Security.ShortName == "Stryker");
                    //Console.Write($"0:{foo0.CostPerShare}({foo0.CostChange}) ");

                    board.AdvanceYear(RollD6(1), RollD6(2)); // Setup the board for buying round 1; return on investments can only be calculated after the first buying round
                    Console.Write($"\nRunning simulation {run} of {simulationRuns}");

                    //var foo = board.BoardSecurities.First(st => st.Security.ShortName == "Stryker");
                    //Console.Write($"1:{foo.CostPerShare}({foo.CostChange}) ");

                    while (board.Year <= _maxRounds)
                    {
                        //Console.Write($"..{board.Year}");
                        board.AdvanceYear(RollD6(1), RollD6(2));
                        foreach (var boardSecurity in board.BoardSecurities)
                        {
                            if (boardSecurity.Security.ShortName == "Stryker")
                            {
                                if (boardSecurity.IsBust)
                                    Console.WriteLine($":B({boardSecurity.CostChange}) ");
                                else
                                    Console.Write($":{boardSecurity.CostPerShare}({boardSecurity.CostChange}) ");
                            }

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
                                if (boardSecurity.IsBust)
                                {
                                    // Because a busted security gets reset right away, we need to ignore the currently posted price/price changes
                                    r = -boardSecurity.CostChange;
                                    rrt = -1;
                                } else
                                {
                                    r = (s * boardSecurity.Security.YieldPer10Shares) + boardSecurity.CostChange;
                                    rrt = r / ((s * boardSecurity.CostPerShare) - boardSecurity.CostChange);
                                }
                            }
                            catch(DivideByZeroException) {
                                rrt = 0;
                                Console.WriteLine($"Attempted to divide by zero ({boardSecurity.Security.Name})\n");
                            } finally
                            {
                                if (Double.IsInfinity(rrt))
                                {
                                    rrt = -1;
                                    Console.WriteLine($"rrt was infinite ({boardSecurity.Security.Name})\n");
                                }
                            }
                            writer.WriteLine($"{run},{boardSecurity.Security.ShortName},{board.Year},{boardSecurity.CostPerShare},{boardSecurity.CostChange},{boardSecurity.IsSplit},{boardSecurity.IsBust},{boardSecurity.Security.YieldPer10Shares},{r},{rrt}");
                        }
                    }
                } 
            }
            Console.WriteLine("====== completed ======");
        }

        public void PlayAiSimulation()
        { 
            var board= new StockBoard(_securities);

            do
            {
                Console.WriteLine($"\n======= YEAR {board.Year} of {_maxRounds} ======");
                AdvanceRound(board);
                Console.WriteLine($"--------------");

                // Take Turns
                foreach (var ai in _players)
                {
                    ai.TakeTurn(board.BoardSecurities, board.Year);
                }

                // Print round end summary
                Console.WriteLine();
                foreach (var player in _players)
                {
                    Console.Write($"({player.TotalWealth(board.BoardSecurities)}) ");
                    player.PrintStatus();
                }

                Console.ReadLine();
            } while (board.Year <= _maxRounds);

                // One last round to see how all investments paid off
                AdvanceRound(board);
            foreach (var player in _players)
            {
                player.CashOut(board.BoardSecurities);
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
                ai.AddYield(board.BoardSecurities);
            }

            // Change prices
            var tempRoll = RollD6(2);
            board.AdvanceYear(RollD6(1), tempRoll);
            if (board.BoardMarket == MarketDirection.Bull)
                Console.WriteLine($"+++ Rolled {tempRoll} on {board.BoardMarket} market +++");
            else
                Console.WriteLine($"--- Rolled {tempRoll} on {board.BoardMarket} market ---");

            foreach (var boardSecurity in board.BoardSecurities)
            {
                // Award securities for players holding split stocks
                if (boardSecurity.IsSplit)
                {
                    Console.WriteLine($"{boardSecurity.Security} split!");
                    foreach (var player in _players) { player.SplitOwnedSecurity(boardSecurity.Security); }
                }

                // Revoke securities for players holding sunk stocks
                if (boardSecurity.IsBust)
                {
                    Console.WriteLine($"{boardSecurity.Security} bust!");
                    foreach (var player in _players) { player.ForfitBustSecurity(boardSecurity.Security); }
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
