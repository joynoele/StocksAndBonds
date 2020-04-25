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
        public int[] YieldPercent { get; private set; }
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
            YieldPercent = new int[maxYears + 1];
            RateOfReturn = new double[maxYears + 1];
            AvgRateOfReturn = new double[maxYears + 1];
            StdDevofRateOfReturn = new double[maxYears + 1];
            MaximumLoss = new int[maxYears + 1];
        }

        public void AddData(Asset asset, int year)
        {
            if (year < OpeningPrice.Length)
            {
                OpeningPrice[year] = asset.CostPerShare;
                YieldRecieved[year] = (int)asset.CollectYieldAmt;
            }

            if (year < 1)
                return;

            // These pieces of data are only known/calculated by data revealed in the following year
            // i.e. involve knowing last year's closing price
            ClosingPrice[year - 1] = asset.CostPerShare;
            PriceDelta[year - 1] = asset.CostChange;
            RateOfReturn[year - 1] = CalculateRateOfReturn(asset, year);
            AvgRateOfReturn[year - 1] = CalculateAvgRateOfReturn(year);
            StdDevofRateOfReturn[year - 1] = CalculateStdDevOfRateOfReturn(year);
            MaximumLoss[year - 1] = CalculateMaximumLoss(year);
            YieldPercent[year - 1] = YieldRecieved[year - 1] / ClosingPrice[year - 1];
        }
        private double CalculateRateOfReturn(Asset asset, int year)
        {
            if (asset.IsBust)
                return -1;

            int splitMultiplier = asset.IsSplit ? 2 : 1;
            return (splitMultiplier * (OpeningPrice[year-1] + PriceDelta[year-1] + YieldRecieved[year-1]) / (double)OpeningPrice[year-1]) - 1;
        }

        private double CalculateAvgRateOfReturn(int year)
        {
            if (!RateOfReturn.Take(year).Any())
                return 0;
            return RateOfReturn.Take(year).Average();
        }
        private double CalculateStdDevOfRateOfReturn(int year)
        {
            if (!RateOfReturn.Take(year).Any())
                return 0;
            return RateOfReturn.Take(year).StandardDeviation();
        }
        private int CalculateMaximumLoss(int year)
        {
            if (!PriceDelta.Take(year).Any())
                return 0;
            return PriceDelta.Min();
        }

        public void PrettyPrint(string assetName, int upToYear)
        {
            // inefficient, but whatevers...
            Console.WriteLine($"---------- {assetName}");
            Console.Write("Year     :\t0\t1\t2\t3\t4\t5\t6\t7\t8\t9\t10");
            Console.Write("\nPrice    :");
            foreach (int price in OpeningPrice.Take(upToYear)) Console.Write($"\t{price}");
            Console.Write("\nPriceD   :");
            // skip the first price change because year 0 does not fundamentally have one
            foreach (int delta in PriceDelta.Take(upToYear)) Console.Write($"\t{delta}");
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
            Console.WriteLine();
        }
    }
}
