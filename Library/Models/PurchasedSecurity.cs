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

        // This is backwards to have, but just in case
        public PurchasedSecurity(Asset asset, int quantity = 0)
        {
            Security = new Security(asset.Id, asset.Name, asset.ShortName, 0);
            Quantity = quantity;
        }
    }
}
