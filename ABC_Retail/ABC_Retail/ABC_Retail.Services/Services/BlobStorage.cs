using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using ABC_Retail.Services;

namespace ABC_Retail.Services
{
    public class BlobStorage
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorage(string connectionString, string containerName = "product-images")
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(imageStream, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Extract blob name from URL
            var uri = new Uri(imageUrl);
            var blobName = Path.GetFileName(uri.LocalPath);

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
