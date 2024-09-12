using System;
using Library.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Library
{
    public class RawSecurities
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public decimal Yield { get; set; } // Percent per 10 shares. i.e. 0.02 = 2% = $20
        public Dictionary<int, int> BullChanges { get; set; }
        public Dictionary<int, int> BearChanges { get; set; }
    }

    public static class SecurityFactory
    {
        private static IList<Security> _securities;
        public static IList<Security> Securities
        {
            get
            {
                if (_securities == null)
                    throw new Exception("Securities not loaded yet; load securities from file");
                return _securities;
            }
        }

        public static BoardSecurities BoardSecurities2 => new BoardSecurities(BoardSecurities);

        private static IDictionary<Security, BoardSecurity> _boardSecurities;
        public static IDictionary<Security, BoardSecurity> BoardSecurities
        {
            get
            {
                if (_securities == null)
                    throw new Exception("Securities not loaded yet; load securities from file first");
                return _boardSecurities;
            }
        }

        /// <summary>
        /// Populate the general securities and the board securities for use later
        /// </summary>
        /// <param name="jsonPath"></param>
        public static void LoadSecurities(string jsonPath)
        {
            RawSecurities[] fromJson;

            // deserialize JSON directly from a file
            // https://www.newtonsoft.com/json/help/html/DeserializeWithJsonSerializerFromFile.htm
            using (StreamReader file = File.OpenText(jsonPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                var temp = (RawSecurities[]?)serializer.Deserialize(file, typeof(RawSecurities[]));
                fromJson = temp ?? throw new Exception("No securities found in file");
            }

            if (_securities == null)
                _securities = new List<Security>();
            if (_boardSecurities == null)
                _boardSecurities = new Dictionary<Security, BoardSecurity>();

            foreach (var @object in fromJson)
            {
                var yield = @object.Yield*100; // because yields are expressed in percents
                var security = new Security(Guid.NewGuid(), @object.Name, @object.ShortName, (int)yield);
                var boardSecurity = new BoardSecurity(yield, @object.BearChanges, @object.BullChanges);
                _securities.Add(security);
                _boardSecurities.Add(security, boardSecurity);
            }
        }

        public static IList<PurchasedSecurity> InitializePlayerPortfolio()
        {
            var emptyPortfolio = new List<PurchasedSecurity>();
            foreach (var sec in Securities)
            {
                emptyPortfolio.Add(new PurchasedSecurity(sec));
            }
            return emptyPortfolio;
        }

        public static StockBoard CreateGameBoard()
        {
            var boardSecurities = new BoardSecurities(_boardSecurities);
            return new StockBoard(boardSecurities);
        }
    }
}
