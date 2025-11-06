using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure; // <-- ADD THIS NAMESPACE FOR HttpRange
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ABC_Retail.Functions
{
    public static class SendToAzureFilesFunction
    {
        [Function("SendToAzureFiles")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("SendToAzureFiles");
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                                       ?? throw new InvalidOperationException("Connection string not found.");

            // --- Configuration ---
            const string ShareName = "logs";
            string fileName = $"file_{Guid.NewGuid()}.txt";
           
            var shareClient = new ShareClient(connectionString, ShareName);
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            // Read content from the request body
            string fileContent = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(fileContent))
            {
                fileContent = $"Default content for {fileName} created at {DateTime.UtcNow}.";
            }

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(fileContent);

            
            await fileClient.CreateAsync(byteArray.Length);

            
            using var stream = new MemoryStream(byteArray);

            
            await fileClient.UploadRangeAsync(new HttpRange(0, byteArray.Length), stream); 

            logger.LogInformation($"Uploaded {fileName} to Azure Files share '{ShareName}'.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync($"File {fileName} uploaded successfully to Azure Files!");
            return response;
        }
    }
}