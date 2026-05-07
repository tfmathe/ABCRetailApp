using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class FileFunction
    {
        private readonly ILogger _logger;
        private readonly ShareClient _shareClient;

        public FileFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FileFunction>();
            var conn = Environment.GetEnvironmentVariable(
                "AzureStorage__ConnectionString");
            _shareClient = new ShareClient(conn, "logs");
            _shareClient.CreateIfNotExists();
        }

        [Function("UploadLogFile")]
        public async Task<IActionResult> UploadLogFile(
            [HttpTrigger(AuthorizationLevel.Function,
                "post", Route = "files/upload")] HttpRequest req)
        {
            _logger.LogInformation("UploadLogFile triggered.");
            var fileName = req.Query["fileName"].ToString();
            if (string.IsNullOrEmpty(fileName))
                fileName = $"log_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
            var dir = _shareClient.GetRootDirectoryClient();
            var fileClient = dir.GetFileClient(fileName);
            await fileClient.CreateAsync(req.Body.Length);
            await fileClient.UploadAsync(req.Body);
            return new OkObjectResult(
                $"Log file '{fileName}' uploaded successfully.");
        }

        [Function("ListLogFiles")]
        public async Task<IActionResult> ListLogFiles(
            [HttpTrigger(AuthorizationLevel.Function,
                "get", Route = "files/list")] HttpRequest req)
        {
            _logger.LogInformation("ListLogFiles triggered.");
            var dir = _shareClient.GetRootDirectoryClient();
            var fileNames = new List<string>();
            await foreach (var item in dir.GetFilesAndDirectoriesAsync())
                if (!item.IsDirectory) fileNames.Add(item.Name);
            return new OkObjectResult(fileNames);
        }

        [Function("DeleteLogFile")]
        public async Task<IActionResult> DeleteLogFile(
            [HttpTrigger(AuthorizationLevel.Function,
                "delete", Route = "files/delete")] HttpRequest req)
        {
            _logger.LogInformation("DeleteLogFile triggered.");
            var fileName = req.Query["fileName"].ToString();
            if (string.IsNullOrEmpty(fileName))
                return new BadRequestObjectResult(
                    "fileName query param required.");
            var dir = _shareClient.GetRootDirectoryClient();
            await dir.GetFileClient(fileName).DeleteIfExistsAsync();
            return new OkObjectResult(
                $"Log file '{fileName}' deleted successfully.");
        }
    }
}