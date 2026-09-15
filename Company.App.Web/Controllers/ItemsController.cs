using Microsoft.AspNetCore.Mvc;
using Refit;
using Company.App.Shared.Contracts;
using Company.App.Shared.DTOs;

namespace Company.App.Web.Controllers;

/// <summary>
/// MVC Controller orchestrating client requests and communicating with ApiService via typed Refit client.
/// Demonstrates CancellationToken cancellation propagation and structured exception handling.
/// </summary>
public class ItemsController : Controller
{
    private readonly IItemsApi _itemsApi;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IItemsApi itemsApi, ILogger<ItemsController> logger)
    {
        _itemsApi = itemsApi;
        _logger = logger;
    }

    /// <summary>
    /// Displays items dashboard with interactive DataTables, Chart.js, Select2, and Leaflet.js components.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching items via Refit API client...");
        try
        {
            var items = await _itemsApi.GetItemsAsync(cancellationToken);
            return View(items);
        }
        catch (ApiException apiEx)
        {
            // Handles HTTP error status responses from ApiService (e.g. 404, 500)
            _logger.LogError(apiEx, "ApiService returned HTTP {StatusCode}: {Message}", apiEx.StatusCode, apiEx.Message);
            ViewBag.ErrorMessage = $"Backend service returned error: {apiEx.StatusCode}";
            return View(Enumerable.Empty<ItemDto>());
        }
        catch (HttpRequestException httpEx)
        {
            // Handles network failure / unreachable backend (handled gracefully by resilience pipeline retries first)
            _logger.LogError(httpEx, "Unable to reach ApiService backend at configured endpoint.");
            ViewBag.ErrorMessage = "Unable to connect to the ApiService backend. Please verify that the API service is running.";
            return View(Enumerable.Empty<ItemDto>());
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Items fetch request was canceled by the client.");
            return View(Enumerable.Empty<ItemDto>());
        }
    }

    /// <summary>
    /// Handles new item submission from dashboard modal form.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ItemDto model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var items = await _itemsApi.GetItemsAsync(cancellationToken);
            return View("Index", items);
        }

        try
        {
            await _itemsApi.CreateItemAsync(model, cancellationToken);
            _logger.LogInformation("Item '{ItemName}' created successfully via Refit client.", model.Name);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx, "Backend rejected item creation with status {StatusCode}.", apiEx.StatusCode);
            ModelState.AddModelError(string.Empty, $"Backend error ({apiEx.StatusCode}): Could not save item.");
            var items = await _itemsApi.GetItemsAsync(cancellationToken);
            return View("Index", items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create item via ApiService.");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred while communicating with the backend.");
            var items = await _itemsApi.GetItemsAsync(cancellationToken);
            return View("Index", items);
        }
    }
}
