using Azure.Data.Tables;
using ABCRetailApp.Models;

namespace ABCRetailApp.Services
{
    public class TableService
    {
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

        public TableService(IConfiguration config)
        {
            var conn = config["AzureStorage:ConnectionString"];
            _customerTable = new TableClient(conn, "CustomerProfiles");
            _productTable = new TableClient(conn, "Products");
            _customerTable.CreateIfNotExists();
            _productTable.CreateIfNotExists();
        }

        // --- Customers ---
        public async Task AddCustomerAsync(CustomerEntity c) =>
            await _customerTable.AddEntityAsync(c);

        public List<CustomerEntity> GetCustomers() =>
            _customerTable.Query<CustomerEntity>().ToList();

        public async Task DeleteCustomerAsync(string partitionKey, string rowKey) =>
            await _customerTable.DeleteEntityAsync(partitionKey, rowKey);

        // --- Products ---
        public async Task AddProductAsync(ProductEntity p) =>
            await _productTable.AddEntityAsync(p);

        public List<ProductEntity> GetProducts() =>
            _productTable.Query<ProductEntity>().ToList();

        public async Task DeleteProductAsync(string partitionKey, string rowKey) =>
            await _productTable.DeleteEntityAsync(partitionKey, rowKey);
    }
}