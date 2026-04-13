using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers
{
    public class TableController : Controller
    {
        private readonly TableService _table;
        public TableController(TableService table) => _table = table;

        // --- Customers ---
        public IActionResult Customers() =>
            View(_table.GetCustomers());

        [HttpPost]
        public async Task<IActionResult> AddCustomer(CustomerEntity customer)
        {
            await _table.AddCustomerAsync(customer);
            TempData["Message"] = $"Customer '{customer.Name}' added.";
            return RedirectToAction("Customers");
        }

        public async Task<IActionResult> DeleteCustomer(
            string partitionKey, string rowKey)
        {
            await _table.DeleteCustomerAsync(partitionKey, rowKey);
            TempData["Message"] = "Customer deleted.";
            return RedirectToAction("Customers");
        }

        // --- Products ---
        public IActionResult Products() =>
            View(_table.GetProducts());

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductEntity product)
        {
            await _table.AddProductAsync(product);
            TempData["Message"] = $"Product '{product.ProductName}' added.";
            return RedirectToAction("Products");
        }

        public async Task<IActionResult> DeleteProduct(
            string partitionKey, string rowKey)
        {
            await _table.DeleteProductAsync(partitionKey, rowKey);
            TempData["Message"] = "Product deleted.";
            return RedirectToAction("Products");
        }
    }
}