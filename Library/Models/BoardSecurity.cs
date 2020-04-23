using System;
using System.Collections.Generic;

namespace Library.Models
{


    public class BoardSecurity
    {
        private const int StartingPrice = 100; // Set point when game is initialized or to reset the price after bottoming out

        public int CostPerShare { get; private set; }
        public bool IsSplit { get; private set; }
        public bool IsBust { get; private set; }
        public Security Security { get; private set; }
        public int CostChange { get; private set; }
        public int CollectYieldAmt { get => !IsBust && CostPerShare > 30 ? this.Security.YieldPer10Shares : 0; } // Securities that fall below a certain value ($30?) cannot have yield collected on them

        private Dictionary<int, int> _bearChange;
        private Dictionary<int, int> _bullChange;

        public BoardSecurity(Security security, Dictionary<int, int> bearChange, Dictionary<int, int> bullChange)
        {
            Security = security;
            _bearChange = bearChange;
            _bullChange = bullChange;
            Initialize();
        }

        // Set the board security to start a new game
        public void Initialize()
        {
            CostPerShare = StartingPrice;
            CostChange = 0;
            IsBust = false;
            IsSplit = false;
        }

        // TODO: lock this method down to only approved classes!!!
        public void AdjustPrice(MarketDirection direction, int diceRoll)
        {
            // Reset the split/bust before calculating the next price
            IsSplit = false;
            IsBust = false;

            int changeValue;
            if (direction.Equals(MarketDirection.Bear))
            {
                _bearChange.TryGetValue(diceRoll, out changeValue);
            }
            else
            {
                _bullChange.TryGetValue(diceRoll, out changeValue);
            }

            // Cost per share busts when it hits 0 or falls below, set the flag but reset price right away
            if (CostPerShare + changeValue <= 0)
            {
                IsBust = true;
                CostPerShare = StartingPrice;
            }

            CostPerShare += changeValue;
            // when cost hits 150 or higher, split price rounding up
            if (!IsBust && CostPerShare >= 150)
            {
                CostPerShare = (int)Math.Round((double)CostPerShare / 2, MidpointRounding.AwayFromZero); // Rounding always goes up; TODO: verify this works correctly with tests or something
                IsSplit = true;
            }
            CostChange = changeValue;
        }
    }

}
