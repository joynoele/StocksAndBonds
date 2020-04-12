using System;
using System.Collections.Generic;

namespace Library.Models
{

    public class BoardSecurity
    {
        public int CostPerShare { get; private set; }
        public bool IsSplit = false;
        public Security Security { get; set; }
        public int CostChange { get; private set; }
        // Securities that fall below a certain value ($30?) cannot have yield collected on them
        public int Yield { get => CostPerShare > 30 ? this.Security.YieldPer10Shares : 0; }

        private Dictionary<int, int> _bearChange;
        private Dictionary<int, int> _bullChange;

        public BoardSecurity(Security security, Dictionary<int, int> bearChange, Dictionary<int, int> bullChange)
        {
            Security = security;
            CostPerShare = 50; // I think the share starts at $50
            _bearChange = bearChange;
            _bullChange = bullChange;
        }

        public void AdjustPrice(MarketDirection direction, int diceRoll)
        {
            // Reset the split before calculating the next price
            IsSplit = false;

            int changeValue;
            if (direction .Equals(MarketDirection.Bear))
            {
                _bearChange.TryGetValue(diceRoll, out changeValue);
            }
            else
            {
                _bullChange.TryGetValue(diceRoll, out changeValue);
            }

            // cost per share may not fall below 0
            if (CostPerShare + changeValue < 0)
            {
                CostChange = -(CostPerShare);
                CostPerShare = 0;
            }
            else
            {
                CostPerShare += changeValue;
                if (CostPerShare >= 150)
                {
                    CostPerShare /= 2; // Round to nearest whole number
                    IsSplit = true;
                }
                CostChange = changeValue;
            }
        }
    }

}
