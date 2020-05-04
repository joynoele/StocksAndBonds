using Library;
using Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security;
using Library.Models.Players;
using Serilog;
using Microsoft.ML;
using Library.ML;
using System.IO;

namespace StocksAndBonds.Console
{
    public class Program
    {
        private static readonly int DefaultStartingBalance = 5000;
        private static int MaxRounds = 10;
        //private const string LOG_FILEPATH = @"C:\tmp\Log\StockPriceSimulation_2020_04_13.csv";

        public static void Main(string[] args)
        {
            SecurityFactory.LoadSecurities("..\\..\\..\\..\\securities.json");

            // Run either the AIs, or a simulation of the game:
            RunAIs(SecurityFactory.BoardSecurities2);
            //SimulateStockPrices(SecurityFactory.BoardSecurities2, 500);

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
            var model = GetRegressionMlModel();
            var Elsa = new MLAi(DefaultStartingBalance, SecurityFactory.BoardSecurities2.Assets, model);
            var Tim = new MaximizeRrtAi(DefaultStartingBalance, MaxRounds, SecurityFactory.BoardSecurities2.Assets);
            var players = new List<IAiPlayer>(){ Tim, Elsa }; // InitializePlayers(DefaultStartingBalance);

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
            var player2 = new YieldAi(startingBalance);
            var players = new List<IAiPlayer>() { player1, player2 };

            foreach (var asset in SecurityFactory.BoardSecurities)
            {
                var security = asset.Key;
                players.Add(new IndexAi(startingBalance, security, false));
                if (security.YieldPer10Shares > 0)
                    players.Add(new IndexAi(startingBalance, security, true));
            }

            return players;
        }

        private static PredictionEngine<Simulation, RateOfReturnPrediction> GetRegressionMlModel()
        {
            MLContext mlContext = new MLContext();

            // If getting error about reflection, add a reference to the Microsoft.ML nuget library in the main MVC project
            // https://stackoverflow.com/questions/51236784/ml-net-fails-to-load-a-model-from-storage-in-mvc-project
            ITransformer trainedModel = mlContext.Model.Load(@"..\..\RrtRegressionModel.zip", out DataViewSchema modelSchema);

            // Create PredictionEngines
            PredictionEngine<Simulation, RateOfReturnPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<Simulation, RateOfReturnPrediction>(trainedModel);

            // >>>>>>>>>>>>>>>>>>>>>>> TEST EXAMPLE
            // 204,Tri - City,7,107,20,False,False,0,0.117538302357605
            //var simulationSample = new Simulation()
            //{
            //    Security = "Tri - City",
            //    Year = 7,
            //    Price = 107,
            //    Delta = 1,
            //    IsSplit = false,
            //    IsBust = false,
            //    Yield = ,
            //    AvgRateOfReturn = 0.117538302357605F // Predict this
            //};
            //RateOfReturnPrediction prediction = predictionEngine.Predict(simulationSample);
            //System.Console.WriteLine($"Predicted:{prediction.RateOfReturn} Actual:{simulationSample.RateOfReturn}");
            // <<<<<<<<<<<<<<<<<<<<<<<<

            return predictionEngine;
        }
    }

}
