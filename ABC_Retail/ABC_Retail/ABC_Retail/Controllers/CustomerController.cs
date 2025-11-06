using Microsoft.AspNetCore.Mvc;
using ABC_Retail.Models;
using ABC_Retail.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


public class CustomerController : Controller
{
    private readonly TableStorage _tableStorage;
    private readonly LogFileService _logFileService;

    public CustomerController(TableStorage tableStorage, LogFileService logFileService)
    {
        _tableStorage = tableStorage;
        _logFileService = logFileService;
    }

    // GET: Customer/Index
    public async Task<IActionResult> Index()
    {
        var customers = new List<Customer>();
        await foreach (var entity in _tableStorage.GetAllCustomersAsync())
        {
            customers.Add(entity);
        }
        return View(customers);
    }

    // GET: Customer/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customer/Create
    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _tableStorage.AddCustomerAsync(customer);
            TempData["SuccessMessage"] = "Customer created successfully!";
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    // GET: Customer/Edit/{id}
    public async Task<IActionResult> Edit(string id)
    {
        var customer = await _tableStorage.GetCustomerAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    // POST: Customer/Edit/{id}
    [HttpPost]
    public async Task<IActionResult> Edit(string id, Customer customer)
    {
        if (id != customer.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            await _tableStorage.UpdateCustomerAsync(customer);
            TempData["SuccessMessage"] = "Customer updated successfully!";
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    // GET: Customer/Delete/{id}
    public async Task<IActionResult> Delete(string id)
    {
        var customer = await _tableStorage.GetCustomerAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    // POST: Customer/DeleteConfirmed/{id}
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _tableStorage.DeleteCustomerAsync(id);
        TempData["SuccessMessage"] = "Customer deleted successfully!";
        return RedirectToAction("Index");
    }
}
