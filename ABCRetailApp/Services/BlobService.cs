using Azure.Storage.Blobs;

namespace ABCRetailApp.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _client;
        private const string Container = "product-images";
        public BlobService(IConfiguration config)
        {
            _client = new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public async Task UploadAsync(IFormFile file)
        {
            var container = _client.GetBlobContainerClient(Container);
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlobClient(file.FileName);
            using var stream = file.OpenReadStream();
            await blob.UploadAsync(stream, overwrite: true);
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            var container = _client.GetBlobContainerClient(Container);
            var urls = new List<string>();
            await foreach (var item in container.GetBlobsAsync())
                urls.Add(container.GetBlobClient(item.Name).Uri.ToString());
            return urls;
        }

        public async Task DeleteBlobAsync(string fileName)
        {
            var container = _client.GetBlobContainerClient(Container);
            await container.GetBlobClient(fileName).DeleteIfExistsAsync();
        }
    }
}