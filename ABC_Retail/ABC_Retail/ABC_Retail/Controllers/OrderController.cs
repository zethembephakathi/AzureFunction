using ABC_Retail.Models;
using ABC_Retail.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABC_Retail.Controllers
{
    public class OrderController : Controller
    {
        private readonly QueueStorage _queueStorage;
        private readonly TableStorage _tableStorage;
        private readonly LogFileService _logFileService;

        public OrderController(QueueStorage queueStorage, TableStorage tableStorage, LogFileService logFileService)
        {
            _queueStorage = queueStorage;
            _tableStorage = tableStorage;
            _logFileService = logFileService;
        }

        // GET: Order/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            await PopulateDropdownsAsync();

            try
            {
                ModelState.Clear();

                order.Id = Guid.NewGuid().ToString();
                double price = 0;
                var product = await _tableStorage.GetProductAsync(order.ProductId);
                if (product != null)
                    price = product.Price * order.Quantity;
                order.TotalPrice = price;

                // Structured message
                var message = $"OrderId:{order.Id};CustomerId:{order.CustomerId};ProductId:{order.ProductId};Quantity:{order.Quantity};TotalPrice:{order.TotalPrice};Action:ProcessingOrder;Status:Started";

                await _queueStorage.SendMessageAsync(message);
                await _tableStorage.AddOrderAsync(order);

                await _logFileService.UploadLogAsync("OrderLogs.txt", $"Created Order {order.Id} at {DateTime.UtcNow}");
                TempData["SuccessMessage"] = "Order submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["OrderMessage"] = "Order could not be created: " + ex.Message;
                return View(order);
            }
        }


        // GET: Order/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = new List<Order>();
            await foreach (var o in _tableStorage.GetAllOrdersAsync())
                orders.Add(o);

            var products = new List<Products>();
            await foreach (var p in _tableStorage.GetAllProductsAsync())
                products.Add(p);

            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                ProductName = products.FirstOrDefault(p => p.Id == order.ProductId)?.Name,
                ProductDescription = products.FirstOrDefault(p => p.Id == order.ProductId)?.Description
            }).ToList();

            await _logFileService.UploadLogAsync("OrderLogs.txt", $"Accessed Order Index at {DateTime.UtcNow}");
            return View(orderViewModels);
        }

        // GET: Order/Queue
        [HttpGet]
        public async Task<IActionResult> Queue()
        {
            var messages = await _queueStorage.PeekMessagesAsync(20);
            return View(messages);
        }

        // Helper: populate dropdowns
        private async Task PopulateDropdownsAsync()
        {
            var customers = new List<Customer>();
            var products = new List<Products>();

            await foreach (var c in _tableStorage.GetAllCustomersAsync())
                customers.Add(c);

            await foreach (var p in _tableStorage.GetAllProductsAsync())
                products.Add(p);

            ViewBag.Customers = customers;
            ViewBag.Products = products;
        }
    }
}
