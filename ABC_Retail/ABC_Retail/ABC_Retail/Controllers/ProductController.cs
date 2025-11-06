using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Models;
using ABC_Retail.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABC_Retail.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableStorage _tableStorage;
        private readonly BlobStorage _blobStorage;
        private readonly LogFileService _logFileService;

        public ProductController(TableStorage tableStorage, BlobStorage blobStorage, LogFileService logFileService)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
            _logFileService = logFileService;
        }

        // GET: Product/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = new List<Products>();
            await foreach (var entity in _tableStorage.GetAllProductsAsync())
            {
                products.Add(entity);
            }
            return View(products);
        }

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products product)
        {
            if (ModelState.IsValid)
            {
                // Handle uploaded image
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        using var stream = file.OpenReadStream();
                        product.ImageUrl = await _blobStorage.UploadImageAsync(stream, file.FileName);
                    }
                }

                // Generate ID if missing
                if (string.IsNullOrEmpty(product.Id))
                {
                    product.Id = Guid.NewGuid().ToString();
                }

                await _tableStorage.AddProductAsync(product);
                await _logFileService.UploadLogAsync("ProductLogs.txt", $"Created Product {product.Id} at {DateTime.UtcNow}");
                TempData["SuccessMessage"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: Product/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _tableStorage.GetProductAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Products product)
        {
            if (ModelState.IsValid)
            {
                // Handle updated image if a new file is uploaded
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length > 0)
                    {
                        // Delete old image first
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            await _blobStorage.DeleteImageAsync(product.ImageUrl);
                        }

                        // Upload new image
                        using var stream = file.OpenReadStream();
                        product.ImageUrl = await _blobStorage.UploadImageAsync(stream, file.FileName);
                    }
                }

                await _tableStorage.UpdateProductAsync(product);
                await _logFileService.UploadLogAsync("ProductLogs.txt", $"Updated Product {product.Id} at {DateTime.UtcNow}");
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: Product/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _tableStorage.GetProductAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _tableStorage.GetProductAsync(id);
            if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                await _blobStorage.DeleteImageAsync(product.ImageUrl); // Delete associated image
            }

            await _tableStorage.DeleteProductAsync(id);
            await _logFileService.UploadLogAsync("ProductLogs.txt", $"Deleted Product {id} at {DateTime.UtcNow}");
            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
