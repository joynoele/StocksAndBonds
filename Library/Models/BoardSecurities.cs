using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    // Read-only interface for an asset/board security
    public readonly struct Asset
    {
        private Security _security { get; }
        private BoardSecurity _boardSecurity { get; }

        public Asset(Security s, BoardSecurity b)
        {
            _security = s;
            _boardSecurity = b;
        }
        public Guid Id => _security.Id;
        public string Name => _security.Name;
        public string ShortName => _security.ShortName;
        public int CostPerShare => _boardSecurity.CostPerShare;
        public bool IsSplit => _boardSecurity.IsSplit;
        public bool IsBust => _boardSecurity.IsBust;
        //public decimal YieldPer10Shares => _boardSecurity.YieldPer10Shares;
        public int CostChange => _boardSecurity.CostChange;
        public decimal CollectYieldAmt => _boardSecurity.CollectYieldAmt;
        public override string ToString() => _security.ToString();
    }

    public class BoardSecurities : IEnumerable<Asset>
    {
        private IDictionary<Security, BoardSecurity> _portfolio;
        private IList<Asset> _assets;
        public IList<Asset> Assets => _assets;

        public BoardSecurities(IDictionary<Security, BoardSecurity> portfolio)
        {
            _portfolio = portfolio;
            _assets = new List<Asset>();
            foreach(var asset in _portfolio)
            {
                _assets.Add(new Asset(asset.Key, asset.Value));
            }
        }

        public void Initialize()
        {
            foreach (var asset in _portfolio)
            {
                asset.Value.Initialize();
            }
        }

        public void AdjustPrice(MarketDirection direction, int diceRoll)
        {
            foreach (var asset in _portfolio)
            {
                asset.Value.AdjustPrice(direction, diceRoll);
            }
        }

        public void PrintStatus()
        {
            foreach (var asset in _portfolio)
            {
                var props = asset.Value;
                var costChange = props.CostChange > 0 ? $"+{props.CostChange}" : $"{props.CostChange}";
                var costPerShare = props.IsSplit ? $"{props.CostPerShare}/{props.CostPerShare * 2}" : $"{ props.CostPerShare}";
                // padding is used to make the names line up
                Console.WriteLine($"{asset.Key.Name.PadRight(30)}\t({costChange})\t{costPerShare}");
            }
        }

        public IEnumerator<Asset> GetEnumerator()
        {
            foreach (var asset in _assets)
            {
                yield return asset;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
