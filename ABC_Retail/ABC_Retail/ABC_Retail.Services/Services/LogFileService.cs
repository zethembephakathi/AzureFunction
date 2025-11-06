using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System.IO;
using System.Threading.Tasks;
using Azure;
using System;
using ABC_Retail.Services;


namespace ABC_Retail.Services
{
    public class LogFileService
    {
        private readonly ShareClient _shareClient;

        public LogFileService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UploadLogAsync(string fileName, string content)
        {
            var directoryClient = _shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(byteArray);
            await fileClient.CreateAsync(byteArray.Length);
            await fileClient.UploadRangeAsync(new HttpRange(0, byteArray.Length), stream);
        }

        // Add this method
        public async Task<int> GetRecentUploadsCountAsync(string fileName = "OrderLogs.txt")
        {
            var directoryClient = _shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
                return 0;

            var download = await fileClient.DownloadAsync();
            using var reader = new StreamReader(download.Value.Content);
            int count = 0;
            while (await reader.ReadLineAsync() != null)
            {
                count++;
            }
            return count;
        }
    }
}
