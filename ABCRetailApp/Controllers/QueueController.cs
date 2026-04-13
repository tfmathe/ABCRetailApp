using ABCRetailApp.Services;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers
{
    public class QueueController : Controller
    {
        private readonly QueueService _queue;
        public QueueController(QueueService queue) => _queue = queue;

        public async Task<IActionResult> Index()
        {
            List<PeekedMessage> messages = await _queue.GetMessagesAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                await _queue.SendMessageAsync(message);
            TempData["Message"] = $"Order message sent: '{message}'";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearQueue()
        {
            await _queue.ClearQueueAsync();
            TempData["Message"] = "Queue cleared.";
            return RedirectToAction("Index");
        }
    }
}