using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksAndBonds.Console.Models
{
    public class Player
    {
        public int Balance { get; private set; }
        public string Name { get; }
        public IList<PurchasedSecurity> OwnedSecurities { get; }

        public Player(string name, int initialBalance)
        {
            Name = name;
            Balance = initialBalance;
            OwnedSecurities = SecurityFactory.InitializePlayerPortfolio();
        }

        public int MaxBuy(BoardSecurity purchaseSecurity)
        {
            int affordableShares = ((Balance / (purchaseSecurity.CostPerShare)/10)*10); // TODO: technically this should be only in groups of 10
            Buy(purchaseSecurity, affordableShares);
            return affordableShares;
        }

        public void Buy(BoardSecurity purchaseSecurity, int purchaseQuantity)
        {
            var cost = purchaseSecurity.CostPerShare * purchaseQuantity;
            if (cost > Balance)
            {
                // Not enough $ to do this transaction
                return;
            }
            // TODO: adjust sellQuantity available in gameplay
            if (OwnedSecurities.First(s => s.Security == purchaseSecurity.Security) == null)
                OwnedSecurities.Add(new PurchasedSecurity(purchaseSecurity.Security));

            OwnedSecurities.First(s => s.Security == purchaseSecurity.Security).Quantity += purchaseQuantity;
            Balance -= cost;

            if (purchaseQuantity > 0)
                System.Console.WriteLine($"{Name} purchased {purchaseQuantity} shares of {purchaseSecurity.Security.Name} for {cost}");
        }

        public void SellAll(IList<BoardSecurity> sellingSecurities)
        {
            foreach (var b in sellingSecurities)
            {
                Sell(b);
            }
        }
        public void Sell(BoardSecurity sellingSecurity)
        {
            // TODO: check if we have this security at all
            var sellQuantity = OwnedSecurities.First(x => x.Security == sellingSecurity.Security).Quantity;
            Sell(sellingSecurity, sellQuantity);
        }

        public void Sell(BoardSecurity sellingSecurity, int sellQuantity)
        {
            // TODO: check if we have this security at all
            if (OwnedSecurities.First(x => x.Security == sellingSecurity.Security).Quantity < sellQuantity)
            {
                // Not enough of this sellingSecurities to do this transaction
                return;
            }
            // TODO: adjust sellQuantity available in gameplay
            OwnedSecurities.First(x => x.Security == sellingSecurity.Security).Quantity -= sellQuantity;
            Balance += sellingSecurity.CostPerShare * sellQuantity;

            if (sellQuantity > 0)
                System.Console.WriteLine($"{Name} sold {sellQuantity} shares of {sellingSecurity.Security.Name} for {sellingSecurity.CostPerShare * sellQuantity}");
        }

        public void PrintStatus()
        {
            System.Console.WriteLine($"{this.Name}: ${this.Balance}");
            foreach (PurchasedSecurity myPortfolio in this.OwnedSecurities)
            {
                if (myPortfolio.Quantity > 0)
                    System.Console.WriteLine($"\t{myPortfolio.Security.Name}\t{myPortfolio.Quantity} shares");
            }
        }

        public void AddYield()
        {
            int yieldTotal = 0;
            foreach (var s in OwnedSecurities)
            {
                if (s.Security.Yield > 0 && s.Quantity > 0)
                {
                    yieldTotal += s.Quantity * s.Security.Yield;
                }
            }

            Balance += yieldTotal;
            if (yieldTotal > 0)
                System.Console.WriteLine($"{Name} received ${yieldTotal} in yield!");
        }

    }
}
