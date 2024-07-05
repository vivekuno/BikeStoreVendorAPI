namespace BikeStoreVendor.API.Model
{
    public class ProductQuantity
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class ProductInfo
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public string Brand { get; set; }
        public string ModelYear { get; set; }
        public double ListPrice { get; set; }
        public string StoreName { get; set; }
    }

    public class ProductCategory
    {
        public string CategoryName { get; set; }
        public ProductInfo ProductInfo { get; set; }
    }
}
