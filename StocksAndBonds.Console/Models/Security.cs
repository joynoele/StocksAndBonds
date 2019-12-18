using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAndBonds.Console.Models
{
    public class Security
    {
        public int Id { get; }
        public string Name { get; }
        public int Yield { get; }

        public Security(int id, string name, int yield = 0)
        {
            Id = id;
            Name = name;
            Yield = yield;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Security s)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(s, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, s))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != s.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (this.Id == s.Id);
        }
    }
}
