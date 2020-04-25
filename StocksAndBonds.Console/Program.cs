using Library;
using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;
using Library.Models.Players;
using Serilog;

namespace StocksAndBonds.Console
{
    public class Program
    {
        private static readonly int DefaultStartingBalance = 5000;
        private static int MaxRounds = 10;
        //private const string LOG_FILEPATH = @"C:\tmp\Log\StockPriceSimulation_2020_04_13.csv";

        public static void Main(string[] args)
        {
            // TODO: Add logging
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console()
            //    .WriteTo.File(LOG_FILEPATH, )
            //    .CreateLogger();

            SecurityFactory.LoadSecurities("..\\..\\..\\..\\securities.json");

            // Run either the AIs, or a simulation of the game:
            RunAIs(SecurityFactory.BoardSecurities2);
            //SimulateStockPrices(SecurityFactory.BoardSecurities2, 10);

            System.Console.ReadLine();
        }

        private static void SimulateStockPrices(BoardSecurities securities, int numberOfSimulations)
        {
            string writePath = @"C:\tmp\StockPriceSimulation_" + DateTime.Now.ToString("yyyy_MM_dd") + ".csv";
            GameSimulation game = new GameSimulation(MaxRounds, securities, new Random(), null);
            game.CaptureSecurityBehavior_Regression(writePath, numberOfSimulations);
        }

        private static void RunAIs(BoardSecurities securities)
        {
            var Tim = new MaximizeRrtAi(DefaultStartingBalance, MaxRounds, SecurityFactory.BoardSecurities2.Assets);
            var Stompy = new YieldAi(DefaultStartingBalance);
            var p1 = new MaximizeRrtAi(DefaultStartingBalance, MaxRounds, SecurityFactory.BoardSecurities2.Assets);
            var p2 = new MostSharesAi(DefaultStartingBalance);
            var players = new List<IAiPlayer>(){ Tim, Stompy, p1, p2 }; // InitializePlayers(DefaultStartingBalance);
            System.Console.WriteLine("===== Starting simulation of 'Stocks & Bonds' =====");
            foreach (var player in players)
            {
                System.Console.WriteLine($"Welcome {player.Name}! `{player.Strategy}`");
            }
            System.Console.Write("Ready???");
            System.Console.ReadLine();

            GameSimulation game = new GameSimulation(MaxRounds, securities, new Random(), players);
            game.PlayAiSimulation();
        }

        private static IList<IAiPlayer> InitializePlayers(int startingBalance)
        {
            var player1 = new MostSharesAi(startingBalance);
            var player2 = new SteadyGrowthAi(startingBalance);
            var player3 = new YieldAi(startingBalance);
            var players = new List<IAiPlayer>() { player1, player2, player3 };

            foreach (var asset in SecurityFactory.BoardSecurities)
            {
                var security = asset.Key;
                players.Add(new IndexAi(startingBalance, security, false));
                if (security.YieldPer10Shares > 0)
                    players.Add(new IndexAi(startingBalance, security, true));
            }

            return players;
        }
    }
}
