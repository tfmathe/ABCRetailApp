using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class BlobFunction
    {
        private readonly ILogger _logger;
        private readonly BlobContainerClient _container;

        public BlobFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BlobFunction>();
            var conn = Environment.GetEnvironmentVariable(
                "AzureStorage__ConnectionString");
            _container = new BlobContainerClient(conn, "product-images");
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        [Function("UploadImage")]
        public async Task<IActionResult> UploadImage(
            [HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "blob/upload")] HttpRequest req)
        {
            _logger.LogInformation("UploadImage triggered.");
            var fileName = req.Query["fileName"].ToString();
            if (string.IsNullOrEmpty(fileName))
                fileName = $"{Guid.NewGuid()}.jpg";
            var blobClient = _container.GetBlobClient(fileName);
            await blobClient.UploadAsync(req.Body, overwrite: true);
            _logger.LogInformation(
                $"Image '{fileName}' uploaded to Blob Storage.");
            return new OkObjectResult(
                $"Image '{fileName}' uploaded. URL: {blobClient.Uri}");
        }

        [Function("ListImages")]
        public async Task<IActionResult> ListImages(
            [HttpTrigger(AuthorizationLevel.Function,
                "get", Route = "blob/list")] HttpRequest req)
        {
            _logger.LogInformation("ListImages triggered.");
            var urls = new List<string>();
            await foreach (var blob in _container.GetBlobsAsync())
                urls.Add(_container.GetBlobClient(
                    blob.Name).Uri.ToString());
            return new OkObjectResult(urls);
        }

        [Function("DeleteImage")]
        public async Task<IActionResult> DeleteImage(
            [HttpTrigger(AuthorizationLevel.Function,
                "delete", Route = "blob/delete")] HttpRequest req)
        {
            _logger.LogInformation("DeleteImage triggered.");
            var fileName = req.Query["fileName"].ToString();
            if (string.IsNullOrEmpty(fileName))
                return new BadRequestObjectResult(
                    "fileName query param required.");
            await _container.GetBlobClient(fileName).DeleteIfExistsAsync();
            return new OkObjectResult(
                $"Image '{fileName}' deleted successfully.");
        }
    }
}