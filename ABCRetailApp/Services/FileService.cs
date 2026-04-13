using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetailApp.Services
{
    public class FileService
    {
        private readonly ShareClient _share;

        public FileService(IConfiguration config)
        {
            _share = new ShareClient(
                config["AzureStorage:ConnectionString"], "logs");
            _share.CreateIfNotExists();
        }

        public async Task UploadLogAsync(IFormFile file)
        {
            var dir = _share.GetRootDirectoryClient();
            var fileClient = dir.GetFileClient(file.FileName);
            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadAsync(stream);
        }

        public async Task<List<ShareFileItem>> ListLogsAsync()
        {
            var dir = _share.GetRootDirectoryClient();
            var files = new List<ShareFileItem>();
            await foreach (var item in dir.GetFilesAndDirectoriesAsync())
                if (!item.IsDirectory) files.Add(item);
            return files;
        }

        // ← THIS METHOD WAS MISSING
        public async Task DeleteLogAsync(string fileName)
        {
            var dir = _share.GetRootDirectoryClient();
            await dir.GetFileClient(fileName).DeleteIfExistsAsync();
        }
    }
}