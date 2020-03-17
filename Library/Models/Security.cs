using System;

namespace Library.Models
{
    public class Security
    {
        public Guid Id { get; }
        public string Name { get; }
        public string ShortName { get; }
        public int YieldPer10Shares { get; }

        public Security(Guid id, string name, string shortName, int yield = 0)
        {
            Id = id;
            Name = name;
            ShortName = shortName;
            YieldPer10Shares = yield;
        }

        public override string ToString()
        {
            return ShortName;
        }
    }
}
