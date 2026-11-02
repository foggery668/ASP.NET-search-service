using System.Net;
using System.Text.Json;
using inizproject.Models;

namespace inizproject.Services
{
    public interface IGoogleSearchService
    {
        Task<List<SearchResultItem>> SearchAsync(string keyword);
    }

    public class GoogleSearchService : IGoogleSearchService
    {
        private readonly HttpClient _httpClient;

        public GoogleSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "inizproject-app/1.0 (Contact: dev@example.com)");
            }
        }

        public async Task<List<SearchResultItem>> SearchAsync(string keyword)
        {
            var results = new List<SearchResultItem>();

            try
            {
                string url = $"https://en.wikipedia.org/w/api.php?action=query&list=search&srsearch={Uri.EscapeDataString(keyword)}&format=json";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return results;
                }

                string jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                var root = doc.RootElement;
                if (root.TryGetProperty("query", out var queryProp) &&
                    queryProp.TryGetProperty("search", out var searchProp))
                {
                    foreach (var item in searchProp.EnumerateArray())
                    {
                        string title = item.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                        string snippet = item.TryGetProperty("snippet", out var s) ? s.GetString() ?? "" : "";

                        snippet = System.Text.RegularExpressions.Regex.Replace(snippet, "<.*?>", string.Empty);

                        string pageUrl = $"https://en.wikipedia.org/wiki/{Uri.EscapeDataString(title.Replace(" ", "_"))}";

                        if (!string.IsNullOrEmpty(title))
                        {
                            results.Add(new SearchResultItem
                            {
                                Title = WebUtility.HtmlDecode(title),
                                Url = pageUrl,
                                Snippet = WebUtility.HtmlDecode(snippet)
                            });
                        }
                    }
                }
            }
            catch
            {
            }

            return results;
        }
    }
}