using System.Diagnostics;
using ABC_Retail.Models;
using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Services;
using Microsoft.Extensions.Logging;

namespace ABC_Retail.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TableStorage _tableStorage;
        private readonly QueueStorage _queueStorage;
        private readonly LogFileService _logFileService;

        public HomeController(ILogger<HomeController> logger, TableStorage tableStorage, QueueStorage queueStorage, LogFileService logFileService)
        {
            _logger = logger;
            _tableStorage = tableStorage;
            _queueStorage = queueStorage;
            _logFileService = logFileService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = new List<ABC_Retail.Models.Customer>();
            var products = new List<ABC_Retail.Models.Products>();
            var ordersInQueue = await _queueStorage.GetMessageCountAsync();
            var recentUploads = await _logFileService.GetRecentUploadsCountAsync();

            await foreach (var c in _tableStorage.GetAllCustomersAsync())
                customers.Add(c);
            await foreach (var p in _tableStorage.GetAllProductsAsync())
                products.Add(p);

            ViewBag.CustomerCount = customers.Count;
            ViewBag.ProductCount = products.Count;
            ViewBag.OrdersInQueue = ordersInQueue;
            ViewBag.RecentUploads = recentUploads;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
