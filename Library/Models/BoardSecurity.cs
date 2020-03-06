using System.Collections.Generic;

namespace Library.Models
{

    public class BoardSecurity
    {
        public int CostPerShare { get; private set; }
        public bool IsSplit = false;
        public Security Security { get; set; }
        public int CostChange { get; private set; }

        private Dictionary<int, int> _bearChange;
        private Dictionary<int, int> _bullChange;

        public BoardSecurity(Security security, Dictionary<int, int> bearChange, Dictionary<int, int> bullChange)
        {
            Security = security;
            CostPerShare = 50; // I think the share starts at $50
            _bearChange = bearChange;
            _bullChange = bullChange;
        }

        public void AdjustPrice(bool isBearMarket, int diceRoll)
        {
            // Reset the split before calculating the next price
            if (IsSplit)
                IsSplit = false; 

            int changeValue = 0;
            if (isBearMarket)
            {
                _bearChange.TryGetValue(diceRoll, out changeValue);
            }
            else
            {
                _bullChange.TryGetValue(diceRoll, out changeValue);
            }
            CostPerShare += changeValue;
            if (CostPerShare >= 150)
            {
                CostPerShare = CostPerShare / 2; // integer division for now
                IsSplit = true;
            }

            CostChange = changeValue;
        }

        // TODO: add method for card changes
    }

}
