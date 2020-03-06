namespace Library.Models
{
    public class Security
    {
        public int Id { get; } // maybe will want this later
        public string Name { get; }
        public int YieldPer10Shares { get; }

        public Security(int id, string name, int yield = 0)
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
