using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using ABC_Retail.Services;
using ABC_Retail.Models;

namespace ABC_Retail.Functions.Functions
{
    public class StoreToTableFunction
    {
        private readonly TableStorage _tableStorage;

        public StoreToTableFunction(TableStorage tableStorage)
        {
            _tableStorage = tableStorage;
        }

        [Function("StoreToTable")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("StoreToTable");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string id = query["id"];
            string name = query["name"];
            string email = query["email"];
            string address = query["address"];

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please pass 'id' and 'name' in the query string.");
                return badResponse;
            }

            await _tableStorage.AddCustomerAsync(new Customer
            {
                Id = id,
                Name = name,
                Email = email,
                Address = address
            });

            logger.LogInformation($"Stored customer {name} into Azure Table.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"Customer {name} stored successfully!");
            return response;
        }
    }
}
