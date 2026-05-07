using Azure.Data.Tables;
using AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class TableFunction
    {
        private readonly ILogger _logger;
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

        public TableFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TableFunction>();
            var conn = Environment.GetEnvironmentVariable(
                "AzureStorage__ConnectionString");
            _customerTable = new TableClient(conn, "CustomerProfiles");
            _productTable = new TableClient(conn, "Products");
            _customerTable.CreateIfNotExists();
            _productTable.CreateIfNotExists();
        }

        [Function("AddCustomer")]
        public async Task<IActionResult> AddCustomer(
            [HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "customers/add")] HttpRequest req)
        {
            _logger.LogInformation("AddCustomer function triggered.");
            var customer = await System.Text.Json.JsonSerializer
                .DeserializeAsync<CustomerEntity>(req.Body);
            customer!.RowKey = Guid.NewGuid().ToString();
            customer.PartitionKey = "Customer";
            await _customerTable.AddEntityAsync(customer);
            return new OkObjectResult(
                $"Customer '{customer.Name}' added successfully.");
        }

        [Function("GetCustomers")]
        public IActionResult GetCustomers(
            [HttpTrigger(AuthorizationLevel.Function,
                "get", Route = "customers")] HttpRequest req)
        {
            _logger.LogInformation("GetCustomers function triggered.");
            var customers = _customerTable
                .Query<CustomerEntity>().ToList();
            return new OkObjectResult(customers);
        }

        [Function("AddProduct")]
        public async Task<IActionResult> AddProduct(
            [HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "products/add")] HttpRequest req)
        {
            _logger.LogInformation("AddProduct function triggered.");
            var product = await System.Text.Json.JsonSerializer
                .DeserializeAsync<ProductEntity>(req.Body);
            product!.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Product";
            await _productTable.AddEntityAsync(product);
            return new OkObjectResult(
                $"Product '{product.ProductName}' added successfully.");
        }

        [Function("GetProducts")]
        public IActionResult GetProducts(
            [HttpTrigger(AuthorizationLevel.Function,
                "get", Route = "products")] HttpRequest req)
        {
            _logger.LogInformation("GetProducts function triggered.");
            var products = _productTable
                .Query<ProductEntity>().ToList();
            return new OkObjectResult(products);
        }
    }
}