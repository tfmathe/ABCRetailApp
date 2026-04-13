using Microsoft.AspNetCore.Mvc;
using ABCRetailApp.Services;

namespace ABCRetailApp.Controllers
{
    public class FileController : Controller
    {
        private readonly FileService _file;
        public FileController(FileService file) => _file = file;

        public async Task<IActionResult> Index()
        {
            var files = await _file.ListLogsAsync();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
                await _file.UploadLogAsync(file);
            TempData["Message"] = $"Log file '{file?.FileName}' uploaded.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            await _file.DeleteLogAsync(fileName);
            TempData["Message"] = $"Log '{fileName}' deleted.";
            return RedirectToAction("Index");
        }
    }
}