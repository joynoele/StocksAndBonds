namespace Library.Models
{
    public class PurchasedSecurity
    {
        public Security Security { get; }
        public int Quantity { get; set; }

        public PurchasedSecurity(Security security, int quantity = 0)
        {
            Security = security;
            Quantity = quantity;
        }
    }
}
