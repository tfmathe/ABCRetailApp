namespace ABCRetailApp.Models
{
    public class TableViewModel
    {
        public List<CustomerEntity> Customers { get; set; } = new();
        public List<ProductEntity> Products { get; set; } = new();
    }
}