using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Models.Players
{
    public interface IPlayer
    {
        string Name { get; }
        string Strategy { get; }
        decimal Balance { get; }
        IList<PurchasedSecurity> Portfolio { get; }
        
        decimal TotalWealth(IList<Asset> securities);
        void AddYield(IList<Asset> securities);
        void PrintStatus();
        decimal CashOut(IList<Asset> securities);
        void SplitOwnedSecurity(Asset security);
        void ForfitBustSecurity(Asset security);
        void SellAll(IList<Asset> sellingSecurities);
    }

    public abstract class Player : IPlayer
    {
        public decimal Balance { get; private set; }
        public string Name { get; }
        public string Strategy { get; }
        public IList<PurchasedSecurity> Portfolio { get; }
        public decimal TotalWealth(IList<Asset> securities)
        {
            var wealth = Balance;
            foreach (var p in Portfolio)
            {
                wealth += securities.First(b => b.Id == p.Security.Id).CostPerShare * p.Quantity;
            }
            return wealth;
        }


        public Player(string name, int initialBalance, string strategy)
        {
            Name = name;
            Strategy = string.IsNullOrWhiteSpace(strategy) ? "...silent waters run deep" : strategy;
            Balance = initialBalance;
            Portfolio = SecurityFactory.InitializePlayerPortfolio();
        }

        public virtual void PrintStatus()
        {
            System.Console.WriteLine($"{this.Name}: ${this.Balance}");
            foreach (PurchasedSecurity myPortfolio in this.Portfolio)
            {
                if (myPortfolio.Quantity > 0)
                    System.Console.WriteLine($"\t{myPortfolio.Security}\t{myPortfolio.Quantity} shares");
            }
        }

        public void AddYield(IList<Asset> boardSecurities)
        {
            foreach (var s in Portfolio)
            {
                if (s.Security.YieldPer10Shares > 0 && s.Quantity > 0)
                {
                    var yield = s.Quantity * s.Security.YieldPer10Shares;
                    System.Console.WriteLine($"{Name} received ${yield} in yield for {s.Quantity} shares of {s.Security}!");
                    Balance += yield;
                }
            }
        }

        protected internal int MaxBuy(Asset purchaseSecurity)
        {
            if (purchaseSecurity.CostPerShare <= 0) return 0; // cannot purchase shares that have $0 valuation

            int affordableShares = (int) Balance / purchaseSecurity.CostPerShare; // I think it truncates partial amounts...
            Buy(purchaseSecurity, affordableShares / 10);
            return affordableShares;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseSecurity"></param>
        /// <param name="purchaseBundle">Set of 10</param>
        protected internal void Buy(Asset purchaseSecurity, int purchaseBundle)
        {
            if (purchaseSecurity.CostPerShare <= 0) return; // cannot purchase shares that have $0 valuation

            var purchaseVolume = purchaseBundle * 10;
            var cost = purchaseSecurity.CostPerShare * purchaseVolume;
            if (cost > Balance)
            {
                // Not enough $ to do this transaction
                return;
            }

            // TODO: adjust sellQuantity available in gameplay

            if (GetAsset(purchaseSecurity) == null)
                Portfolio.Add(new PurchasedSecurity(purchaseSecurity));

            Portfolio.First(s => s.Security.Id == purchaseSecurity.Id).Quantity += purchaseVolume;
            Balance -= cost;

            if (purchaseVolume > 0)
                System.Console.WriteLine($"{Name} purchased {purchaseVolume} shares of {purchaseSecurity} for {cost}");
        }

        // use public until I figure out the right access level for decendents farther down
        public void SellAll(IList<Asset> sellingSecurities)
        {
            foreach (var b in sellingSecurities)
            {
                Sell(b);
            }
        }
        protected internal void Sell(Asset sellingSecurity)
        {
            var sellQuantity = GetAsset(sellingSecurity).Quantity;
            Sell(sellingSecurity, sellQuantity);
        }

        protected internal void Sell(Asset sellingSecurity, int sellQuantity)
        {
            // TODO: check if we have this security at all
            if (GetAsset(sellingSecurity).Quantity < sellQuantity)
            {
                // Not enough of this sellingSecurities to do this transaction
                return;
            }
            // TODO: adjust sellQuantity available in gameplay
            GetAsset(sellingSecurity).Quantity -= sellQuantity;
            Balance += sellingSecurity.CostPerShare * sellQuantity;

            if (sellQuantity > 0)
                System.Console.WriteLine($"{Name} sold {sellQuantity} shares of {sellingSecurity} for {sellingSecurity.CostPerShare * sellQuantity}");
        }

        public decimal CashOut(IList<Asset> securities)
        {
            SellAll(securities);
            return Balance;
        }

        public void SplitOwnedSecurity(Asset asset)
        {
            Portfolio.First(s => s.Security.Id == asset.Id).Quantity *= 2;
        }

        public void ForfitBustSecurity(Asset asset)
        {
            Portfolio.First(s => s.Security.Id == asset.Id).Quantity = 0;
        }

        protected internal PurchasedSecurity GetAsset(Asset findAsset)
        {
            return Portfolio.Where(x => x.Security.Id == findAsset.Id).FirstOrDefault();
        }

        protected internal PurchasedSecurity GetAsset(Security findSecurity)
        {
            return Portfolio.Where(x => x.Security.Id == findSecurity.Id).FirstOrDefault();
        }
    }
}

