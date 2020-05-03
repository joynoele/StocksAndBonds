using System;
using System.Text;
using Microsoft.ML.Data;

namespace Library.ML
{
    public class Simulation
    {
        // NOTE: numerical columns must be float or single types
        [LoadColumn(0)]
        public float Run;

        [LoadColumn(1)]
        public string Security;

        [LoadColumn(2)]
        public float Year;

        [LoadColumn(3)]
        public float Price;

        [LoadColumn(4)]
        public float Delta;

        [LoadColumn(5)]
        public bool IsSplit;

        [LoadColumn(6)]
        public bool IsBust;

        [LoadColumn(7)]
        public float Yield;

        [LoadColumn(8)]
        public float ReturnOnInvestment;

        [LoadColumn(9)]
        [ColumnName("Label")]
        public float RateOfReturn;
    }

    public class RateOfReturnPrediction
    {
        [ColumnName("Score")]
        public float RateOfReturn;
    }
}
