using Azure.Data.Tables;
using ABC_Retail.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABC_Retail.Services
{

    

    public class TableStorage
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

        public TableStorage(object tableStorageConnection)
        {

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? throw new InvalidOperationException("AzureWebJobsStorage is not set in environment variables.");


            _tableServiceClient = new TableServiceClient(connectionString);
            _customerTable = _tableServiceClient.GetTableClient("Customers");
            _customerTable.CreateIfNotExists();
            _productTable = _tableServiceClient.GetTableClient("Products");
            _productTable.CreateIfNotExists();
        }

        // Customer CRUD
        public async Task AddCustomerAsync(Customer customer)
        {
            var entity = new TableEntity("Customer", customer.Id)
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "Address", customer.Address }
            };
            await _customerTable.AddEntityAsync(entity);
        }

        public async Task<Customer?> GetCustomerAsync(string id)
        {
            var response = await _customerTable.GetEntityIfExistsAsync<TableEntity>("Customer", id);
            if (response.HasValue)
            {
                var entity = response.Value;
                return new Customer
                {
                    Id = entity.RowKey,
                    Name = entity.GetString("Name"),
                    Email = entity.GetString("Email"),
                    Address = entity.GetString("Address")
                };
            }
            return null;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var entity = new TableEntity("Customer", customer.Id)
            {
                { "Name", customer.Name },
                { "Email", customer.Email },
                { "Address", customer.Address }
            };
            await _customerTable.UpsertEntityAsync(entity);
        }

        public async Task DeleteCustomerAsync(string id)
        {
            await _customerTable.DeleteEntityAsync("Customer", id);
        }

        public async IAsyncEnumerable<Customer> GetAllCustomersAsync()
        {
            await foreach (var entity in _customerTable.QueryAsync<TableEntity>(e => e.PartitionKey == "Customer"))
            {
                yield return new Customer
                {
                    Id = entity.RowKey,
                    Name = entity.GetString("Name"),
                    Email = entity.GetString("Email"),
                    Address = entity.GetString("Address")
                };
            }
        }

        // Product CRUD
        public async Task AddProductAsync(Products product)
        {
            var entity = new TableEntity("Product", product.Id)
            {
                { "Name", product.Name },
                { "Description", product.Description },
                { "Price", product.Price },
                { "ImageUrl", product.ImageUrl }
            };
            await _productTable.AddEntityAsync(entity);
        }

        public async Task<Products?> GetProductAsync(string id)
        {
            var response = await _productTable.GetEntityIfExistsAsync<TableEntity>("Product", id);
            if (response.HasValue)
            {
                var entity = response.Value;
                return new Products
                {
                    Id = entity.RowKey,
                    Name = entity.GetString("Name"),
                    Description = entity.GetString("Description"),
                    Price = entity.GetDouble("Price") ?? 0,
                    ImageUrl = entity.GetString("ImageUrl")
                };
            }
            return null;
        }

        public async Task UpdateProductAsync(Products product)
        {
            var entity = new TableEntity("Product", product.Id)
            {
                { "Name", product.Name },
                { "Description", product.Description },
                { "Price", product.Price },
                { "ImageUrl", product.ImageUrl }
            };
            await _productTable.UpsertEntityAsync(entity);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _productTable.DeleteEntityAsync("Product", id);
        }

        public async IAsyncEnumerable<Products> GetAllProductsAsync()
        {
            await foreach (var entity in _productTable.QueryAsync<TableEntity>(e => e.PartitionKey == "Product"))
            {
                yield return new Products
                {
                    Id = entity.RowKey,
                    Name = entity.GetString("Name"),
                    Description = entity.GetString("Description"),
                    Price = entity.GetDouble("Price") ?? 0,
                    ImageUrl = entity.GetString("ImageUrl")
                };
            }
        }

        // Order CRUD
        public async Task AddOrderAsync(Order order)
        {
            var entity = new TableEntity("Order", order.Id)
            {
                { "CustomerId", order.CustomerId },
                { "ProductId", order.ProductId },
                { "Quantity", order.Quantity },
                { "TotalPrice", order.TotalPrice }
            };
            var orderTable = _tableServiceClient.GetTableClient("Orders");
            await orderTable.CreateIfNotExistsAsync();
            await orderTable.AddEntityAsync(entity);
        }

        public async Task<Order?> GetOrderAsync(string id)
        {
            var orderTable = _tableServiceClient.GetTableClient("Orders");
            var response = await orderTable.GetEntityIfExistsAsync<Azure.Data.Tables.TableEntity>("Order", id);
            if (response.HasValue)
            {
                var entity = response.Value;
                return new Order
                {
                    Id = entity.RowKey,
                    CustomerId = entity.GetString("CustomerId"),
                    ProductId = entity.GetString("ProductId"),
                    Quantity = entity.GetInt32("Quantity") ?? 0,
                    TotalPrice = entity.GetDouble("TotalPrice") ?? 0
                };
            }
            return null;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var orderTable = _tableServiceClient.GetTableClient("Orders");
            var entity = new Azure.Data.Tables.TableEntity("Order", order.Id)
            {
                { "CustomerId", order.CustomerId },
                { "ProductId", order.ProductId },
                { "Quantity", order.Quantity },
                { "TotalPrice", order.TotalPrice }
            };
            await orderTable.UpsertEntityAsync(entity);
        }

        public async Task DeleteOrderAsync(string id)
        {
            var orderTable = _tableServiceClient.GetTableClient("Orders");
            await orderTable.DeleteEntityAsync("Order", id);
        }

        public async IAsyncEnumerable<Order> GetAllOrdersAsync()
        {
            var orderTable = _tableServiceClient.GetTableClient("Orders");
            await orderTable.CreateIfNotExistsAsync();
            await foreach (var entity in orderTable.QueryAsync<TableEntity>(e => e.PartitionKey == "Order"))
            {
                yield return new Order
                {
                    Id = entity.RowKey,
                    CustomerId = entity.GetString("CustomerId"),
                    ProductId = entity.GetString("ProductId"),
                    Quantity = entity.GetInt32("Quantity") ?? 0,
                    TotalPrice = entity.GetDouble("TotalPrice") ?? 0
                };
            }
        }
    }
}
