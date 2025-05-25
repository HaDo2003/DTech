using DTech.DAO;
using DTech.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace DTech.Controllers
{
    public class SearchController(
        ProductDAO productDAO
    ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Search(string query, string? sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // Save search term for logged in users
                await productDAO.SaveSearchTermAsync(query, userId);
            }
            else
            {
                // Save search term for non-logged-in users in session
                SaveSearchTermForAnonymousUser(query);
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            var products = await productDAO.SearchProductsAsync(query);
            if (products == null || products.Count == 0)
            {
                ViewBag.Message = "No products found matching your search.";
                products = [];
            }

            products = await productDAO.SortProducts(products, sortOrder);
            ViewBag.Query = query;
            ViewBag.SortOrder = sortOrder;
            ViewBag.Title = "Search result for: " + query;

            // Pass search history to view
            ViewBag.SearchHistory = await GetSearchHistoryForView();

            return View(products);
        }

        // Add this method to get search history
        private async Task<List<SearchTerm>> GetSearchHistoryForView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // Get from database for logged-in users
                var searchHistory = await productDAO.GetUserSearchHistoryAsync(userId);
                if (searchHistory == null)
                {
                    return [];
                }
                else
                {
                    var searchTerms = searchHistory.Select(s => new SearchTerm
                    {
                        Query = s.SearchTerm,
                        SearchDate = s.SearchDate
                    }).ToList();
                    return searchTerms;
                }
               
            }
            else
            {
                // Get from session for anonymous users
                var searchTermsJson = HttpContext.Session.GetString("SearchTerms");
                if (string.IsNullOrEmpty(searchTermsJson))
                {
                    return new List<SearchTerm>();
                }
                return JsonSerializer.Deserialize<List<SearchTerm>>(searchTermsJson) ?? new List<SearchTerm>();
            }
        }

        // Add this action to get search history via AJAX
        [HttpGet]
        public async Task<IActionResult> GetSearchHistory()
        {
            var searchHistory = await GetSearchHistoryForView();
            return Json(searchHistory);
        }

        // Add this action to clear search history
        [HttpPost]
        public async Task<IActionResult> ClearSearchHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // Clear from database for logged-in users
                await productDAO.ClearUserSearchHistoryAsync(userId);
            }
            else
            {
                // Clear from session for anonymous users
                HttpContext.Session.Remove("SearchTerms");
            }

            return Json(new { success = true });
        }

        // Add this action to remove individual search term
        [HttpPost]
        public async Task<IActionResult> RemoveSearchTerm(string query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // Remove from database for logged-in users
                await productDAO.RemoveSearchTermAsync(query, userId);
            }
            else
            {
                // Remove from session for anonymous users
                var searchTermsJson = HttpContext.Session.GetString("SearchTerms");
                if (!string.IsNullOrEmpty(searchTermsJson))
                {
                    var searchTerms = JsonSerializer.Deserialize<List<SearchTerm>>(searchTermsJson) ?? new List<SearchTerm>();
                    searchTerms.RemoveAll(s => s.Query.Equals(query, StringComparison.OrdinalIgnoreCase));
                    HttpContext.Session.SetString("SearchTerms", JsonSerializer.Serialize(searchTerms));
                }
            }

            return Json(new { success = true });
        }

        private void SaveSearchTermForAnonymousUser(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            var searchTermsCookie = Request.Cookies["SearchTerms"];
            List<SearchTerm> searchTerms;

            if (string.IsNullOrEmpty(searchTermsCookie))
            {
                searchTerms = new List<SearchTerm>();
            }
            else
            {
                searchTerms = JsonSerializer.Deserialize<List<SearchTerm>>(searchTermsCookie) ?? new List<SearchTerm>();
            }

            searchTerms.Add(new SearchTerm
            {
                Query = query,
                SearchDate = DateTime.Now
            });

            // Keep only last 10 searches
            if (searchTerms.Count > 10)
            {
                searchTerms = searchTerms.OrderByDescending(s => s.SearchDate).Take(10).ToList();
            }

            var updatedJson = JsonSerializer.Serialize(searchTerms);
            Response.Cookies.Append("SearchTerms", updatedJson, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30), // Persists for 30 days
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
