using System;

namespace Library.Models
{
    public class Security
    {
        public Guid Id { get; }
        public string Name { get; }
        public int YieldPer10Shares { get; }

        public Security(Guid id, string name, int yield = 0)
        {
            Id = id;
            Name = name;
            YieldPer10Shares = yield;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
