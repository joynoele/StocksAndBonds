using System.Collections.Generic;
using System.Linq;

namespace Library.Models.Players
{
    public interface IPlayer
    {
        string Name { get; }
        string Strategy { get; }
        int Balance { get; }
        IList<PurchasedSecurity> Portfolio { get; }
        
        int TotalWealth(IList<BoardSecurity> securities);
        void AddYield(IList<BoardSecurity> securities);
        void PrintStatus();
        int CashOut(IList<BoardSecurity> securities);
        void SplitOwnedSecurity(Security security);
        void ForfitBustSecurity(Security security);
    }

    public abstract class Player : IPlayer
    {
        public int Balance { get; private set; }
        public string Name { get; }
        public string Strategy { get; }
        public IList<PurchasedSecurity> Portfolio { get; }
        public int TotalWealth(IList<BoardSecurity> securities)
        {
            var wealth = Balance;
            foreach (var p in Portfolio)
            {
                wealth += securities.First(b => b.Security == p.Security).CostPerShare * p.Quantity;
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

        public void AddYield(IList<BoardSecurity> boardSecurities)
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

        protected int MaxBuy(BoardSecurity purchaseSecurity)
        {
            if (purchaseSecurity.CostPerShare <= 0) return 0; // cannot purchase shares that have $0 valuation

            int affordableShares = Balance / (purchaseSecurity.CostPerShare);
            Buy(purchaseSecurity, affordableShares / 10);
            return affordableShares;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseSecurity"></param>
        /// <param name="purchaseBundle">Set of 10</param>
        protected void Buy(BoardSecurity purchaseSecurity, int purchaseBundle)
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
            if (Portfolio.First(s => s.Security == purchaseSecurity.Security) == null)
                Portfolio.Add(new PurchasedSecurity(purchaseSecurity.Security));

            Portfolio.First(s => s.Security == purchaseSecurity.Security).Quantity += purchaseVolume;
            Balance -= cost;

            if (purchaseVolume > 0)
                System.Console.WriteLine($"{Name} purchased {purchaseVolume} shares of {purchaseSecurity.Security} for {cost}");

        }

        protected void SellAll(IList<BoardSecurity> sellingSecurities)
        {
            foreach (var b in sellingSecurities)
            {
                Sell(b);
            }
        }
        protected void Sell(BoardSecurity sellingSecurity)
        {
            var sellQuantity = Portfolio.First(x => x.Security == sellingSecurity.Security).Quantity;
            Sell(sellingSecurity, sellQuantity);
        }

        protected void Sell(BoardSecurity sellingSecurity, int sellQuantity)
        {
            // TODO: check if we have this security at all
            if (Portfolio.First(x => x.Security == sellingSecurity.Security).Quantity < sellQuantity)
            {
                // Not enough of this sellingSecurities to do this transaction
                return;
            }
            // TODO: adjust sellQuantity available in gameplay
            Portfolio.First(x => x.Security == sellingSecurity.Security).Quantity -= sellQuantity;
            Balance += sellingSecurity.CostPerShare * sellQuantity;

            if (sellQuantity > 0)
                System.Console.WriteLine($"{Name} sold {sellQuantity} shares of {sellingSecurity.Security} for {sellingSecurity.CostPerShare * sellQuantity}");
        }

        public int CashOut(IList<BoardSecurity> securities)
        {
            SellAll(securities);
            return Balance;
        }

        public void SplitOwnedSecurity(Security security)
        {
            Portfolio.First(s => s.Security == security).Quantity *= 2;
        }

        public void ForfitBustSecurity(Security security)
        {
            Portfolio.First(s => s.Security == security).Quantity = 0;
        }
    }
}

