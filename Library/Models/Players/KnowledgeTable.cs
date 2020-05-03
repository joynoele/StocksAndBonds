using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Statistics;

namespace Library.Models.Players
{

    // Each "table" is a single security tracked over time
    // Each index of the arrays track the "opening" price of the share, and the change enacted after the year is closed out.
    // However, the "closing"/delta price is not known until the start of the following year. Hence, some attributes populate last year's data
    public class KnowledgeTable
    {
        // Index of the arrays indicate the year
        public int[] OpeningPrice { get; private set; }
        public int[] ClosingPrice { get; private set; }
        public int[] PriceDelta { get; private set; }
        public int[] YieldRecieved { get; private set; }
        public double[] YieldPercent { get; private set; }
        public double[] RateOfReturn { get; private set; }
        public double[] AvgRateOfReturn { get; private set; }
        public double[] StdDevofRateOfReturn { get; private set; }
        public int[] MaximumLoss { get; private set; }

        public KnowledgeTable(int maxYears)
        {
            // the +1 is to create holders for the final closing value
            OpeningPrice = new int[maxYears + 1];
            ClosingPrice = new int[maxYears + 1];
            PriceDelta = new int[maxYears + 1];
            YieldRecieved = new int[maxYears + 1];
            YieldPercent = new double[maxYears + 1];
            RateOfReturn = new double[maxYears + 1];
            AvgRateOfReturn = new double[maxYears + 1];
            StdDevofRateOfReturn = new double[maxYears + 1];
            MaximumLoss = new int[maxYears + 1];
            OpeningPrice[0] = 100; // Per the game rules, this is a constant.
        }

        public void AddData(Asset asset, int currentYear)
        {
            int index = currentYear - 1; // allowance for the off-by-one in indexing vs year
            // index fills in information as if the market has open and closed for the year
            // and we are making calculations for what to buy at "closing" price

            // The order of these calculations is importaint since latter depend on earlier values being populated
            
            ClosingPrice[index] = asset.CostPerShare;
            PriceDelta[index] = asset.CostChange;
            YieldRecieved[index] = (int)asset.CollectYieldAmt;
            RateOfReturn[index] = CalculateRateOfReturn(asset, currentYear);
            AvgRateOfReturn[index] = CalculateAvgRateOfReturn(currentYear);
            StdDevofRateOfReturn[index] = CalculateStdDevOfRateOfReturn(currentYear);
            MaximumLoss[index] = CalculateMaximumLoss(currentYear);
            YieldPercent[index] = (double)YieldRecieved[index] / ClosingPrice[index];
            if (currentYear < OpeningPrice.Length) OpeningPrice[currentYear] = asset.CostPerShare;
        }
        private double CalculateRateOfReturn(Asset asset, int populateYear)
        {
            if (asset.IsBust)
                return -1;

            int splitMultiplier = asset.IsSplit ? 2 : 1;
            return (splitMultiplier * (OpeningPrice[populateYear - 1] + PriceDelta[populateYear - 1] + YieldRecieved[populateYear - 1]) / (double)OpeningPrice[populateYear - 1]) - 1;
        }

        private double CalculateAvgRateOfReturn(int populateYear)
        {
            if (!RateOfReturn.Take(populateYear).Any())
                return 0;
            return RateOfReturn.Take(populateYear).Average();
        }
        private double CalculateStdDevOfRateOfReturn(int populateYear)
        {
            if (!RateOfReturn.Take(populateYear).Any())
                return 0;
            return RateOfReturn.Take(populateYear).StandardDeviation();
        }
        private int CalculateMaximumLoss(int populateYear)
        {
            if (!PriceDelta.Take(populateYear).Any())
                return 0;
            return PriceDelta.Min();
        }

        public void PrettyPrint(string assetName, int upToYear)
        {
            // inefficient, but whatevers...
            Console.WriteLine($"---------- {assetName}");
            Console.Write("Year     :\t0\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10");
            Console.Write("\nOPrice   :");
            foreach (int oprice in OpeningPrice.Take(upToYear)) Console.Write($"\t{oprice}");
            Console.Write("\nDelta   :");
            foreach (int delta in PriceDelta.Take(upToYear)) Console.Write($"\t{delta}");
            Console.Write("\nCPrice   :");
            foreach (int cprice in ClosingPrice.Take(upToYear)) Console.Write($"\t{cprice}");
            Console.Write("\nYield    :");
            foreach (int yield in YieldRecieved.Take(upToYear)) Console.Write($"\t{yield}");
            Console.Write("\nRrt      :");
            foreach (double rrt in RateOfReturn.Take(upToYear)) Console.Write($"\t{rrt:p2}");
            Console.Write("\nAvgRrt   :");
            foreach (double avgRrt in AvgRateOfReturn.Take(upToYear)) Console.Write($"\t{avgRrt:p2}");
            Console.Write("\nStdDevRrt:");
            foreach (double stdDevRrt in StdDevofRateOfReturn.Take(upToYear)) Console.Write($"\t{stdDevRrt:p2}");
            Console.Write("\nMaxLoss  :");
            foreach (int maxLoss in MaximumLoss.Take(upToYear)) Console.Write($"\t{maxLoss}");
            Console.Write("\nMaxLoss  :");
            foreach (int maxLoss in MaximumLoss.Take(upToYear)) Console.Write($"\t{maxLoss}");
            // TODO: add in avg price delta, std dev of price delta, and yield %
            Console.WriteLine();
        }
    }
}
