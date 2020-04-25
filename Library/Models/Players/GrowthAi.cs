using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Models.Players
{
    public abstract class GrowthAi : Player
    {
        private static string strategy = "Optimization is the root of all profit";
        protected IDictionary<Asset, KnowledgeTable> AnalysisTables { get; private set; }

        public GrowthAi(int initialBalance, string name, string altStrategy, int maxYears, IEnumerable<Asset> assetsInPlay) 
            : base("Growth"+ name, initialBalance, altStrategy ?? strategy)
        {
            AnalysisTables = new Dictionary<Asset, KnowledgeTable>();
            foreach (var asset in assetsInPlay)
            {
                AnalysisTables.Add(asset, new KnowledgeTable(maxYears));
            }
        }

        //public abstract void TakeTurn(IList<Asset> assets, int year)
        //{
        //    UpdateLedger(assets, year);

        //    // evaluate the securities based on calculations in the ledger
        //    foreach (var table in AnalysisTables)
        //    {
        //        var asset = table.Key;
        //        var data = table.Value;
        //        // TODO: put logic here
        //    }

        //    // TODO: buy/sell
        //}

        //public override void PrintStatus()
        //{
        //    Console.WriteLine($"{Name}: ${Balance} {GetAsset(IndexSecurity).Quantity} shares");
        //}

        protected void UpdateLedger(IList<Asset> updatedAssets, int year)
        {
            foreach(var table in AnalysisTables)
            {
                table.Value.AddData(updatedAssets.First(a => a.Id == table.Key.Id), year);
            }
        }

    }
}
