using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using System.Linq;
using Library.ML;

namespace Library.Models.Players
{
    public interface IMLAi : IAiPlayer
    {

    }

    public class MLAi : Player, IMLAi
    {
        private static string strategy = "Drawing on years of experience";
        private PredictionEngine<Simulation, RateOfReturnPrediction> PredictionEngine;
        private IDictionary<Asset, float?> GuessBook; // nullable in case the prediction engine does not have a prediction

        public MLAi(int initialBalance, IEnumerable<Asset> assetsInPlay, PredictionEngine<Simulation, RateOfReturnPrediction> predictionEngine)
            : base("MachineLearning", initialBalance, strategy)
        {
            PredictionEngine = predictionEngine;
            GuessBook = new Dictionary<Asset, float?>();
            foreach(var a in assetsInPlay)
            {
                GuessBook.Add(a, null);
            }
        }

        public void TakeTurn(IList<Asset> securities, int year)
        {
            foreach(var asset in securities)
            {
                if (GuessBook.ContainsKey(asset)) {
                    var situation = new Simulation()
                    {
                        Year = year,
                        Security = asset.ShortName,
                        Price = asset.CostPerShare,
                        Delta = asset.CostChange,
                        IsSplit = asset.IsSplit,
                        IsBust = asset.IsBust,
                        Yield = (float)asset.CollectYieldAmt,
                    };
                    var prediction = PredictionEngine.Predict(situation);
                    GuessBook[asset] = prediction.RateOfReturn;
                    //System.Console.WriteLine($"\t{asset} Predicted:{prediction.RateOfReturn:p2}");
                }
            }
            // For assets that are not in the GuessBook, we should assign an averate rate of return estimate instead

            // TODO: call the cashout or sell all instead once access modifiers are figured out
            foreach (var b in securities)
            {
                Sell(b);
            }
            var securitiesByGrowthHistory = GuessBook.OrderByDescending(x => x.Value);

            Console.WriteLine($"{this.Name}'s top 3 picks: " +
                $"{securitiesByGrowthHistory.First().Key} ({securitiesByGrowthHistory.First().Value:p2}), " +
                $"{securitiesByGrowthHistory.Skip(1).Take(1).First().Key} ({securitiesByGrowthHistory.Skip(1).Take(1).First().Value:p2}), " +
                $"{securitiesByGrowthHistory.Skip(2).Take(1).First().Key} ({securitiesByGrowthHistory.Skip(2).Take(1).First().Value:p2})");
            foreach (var goodBuy in securitiesByGrowthHistory)
            {
                MaxBuy(goodBuy.Key);
            }
        }
    }



}
