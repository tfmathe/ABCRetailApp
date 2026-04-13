using Microsoft.AspNetCore.Mvc;
using ABCRetailApp.Services;

public class BlobController : Controller
{
    private readonly BlobService _blob;
    public BlobController(BlobService blob) => _blob = blob;

    public async Task<IActionResult> Index() =>
        View(await _blob.ListBlobsAsync());

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null) await _blob.UploadAsync(file);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string fileName)
    {
        await _blob.DeleteBlobAsync(fileName);
        return RedirectToAction("Index");
    }
}