using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Models;
using ABC_Retail.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABC_Retail.Controllers
{
    public class SearchController : Controller
    {
        private readonly TableStorage _tableStorage;

        public SearchController(TableStorage tableStorage)
        {
            _tableStorage = tableStorage;
        }

        // Index action to load main search page
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // loads Views/Search/Index.cshtml
        }

        // Search Customer by ID
        [HttpGet("Search/Customer")]
        public async Task<IActionResult> CustomerById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Customer ID is required");

            var customer = await _tableStorage.GetCustomerAsync(id);
            if (customer == null)
                return NotFound("Customer not found");

            return View("Index", customer); 
        }

        // Search Customer by Name
        [HttpGet]
        public async Task<IActionResult> CustomerByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            var results = new List<Customer>();
            await foreach (var c in _tableStorage.GetAllCustomersAsync())
            {
                if (c.Name.ToLower().Contains(name.ToLower()))
                    results.Add(c);
            }

            return View("Index", results); 
        }

        // Search Product by ID
        [HttpGet("Search/Product")]
        public async Task<IActionResult> ProductById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Product ID is required");

            var product = await _tableStorage.GetProductAsync(id);
            if (product == null)
                return NotFound("Product not found");

            return View("Index", product);
        }

        // Search Product by Name
        [HttpGet]
        public async Task<IActionResult> ProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            var results = new List<Products>();
            await foreach (var p in _tableStorage.GetAllProductsAsync())
            {
                if (p.Name.ToLower().Contains(name.ToLower()))
                    results.Add(p);
            }

            return View("Index", results); 
        }

        // Search Order by ID
        [HttpGet("Search/Order")]
        public async Task<IActionResult> OrderById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Order ID is required");

            var order = await _tableStorage.GetOrderAsync(id);
            if (order == null)
                return NotFound("Order not found");

            return View("Index", order); 
        }

        // Search Orders by Customer ID
        [HttpGet]
        public async Task<IActionResult> OrdersByCustomer(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return BadRequest("Customer ID is required");

            var results = new List<Order>();
            await foreach (var o in _tableStorage.GetAllOrdersAsync())
            {
                if (o.CustomerId == customerId)
                    results.Add(o);
            }

            return View("Index", results); 
        }
    }
}
