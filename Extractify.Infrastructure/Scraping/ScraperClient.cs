using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extractify.Infrastructure.Models;
using Extractify.Infrastructure.Scraping;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Extractify.Infrastructure.Scraping
{
    public class ScraperClient : IScraperClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScraperClient> _logger;

        public ScraperClient(HttpClient httpClient, ILogger<ScraperClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ScrapedItem>> ScrapeAsync(string url, string? selector = null, string? imageSelector = null)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var data = new List<ScrapedItem>();

                // Default CSS selectors if none provided
                selector ??= "h1,h2,h3,p";
                imageSelector ??= "img[src]";

                // Convert CSS selectors to XPath
                var textXPath = CssToXPath(selector);
                var imageXPath = CssToXPath(imageSelector);

                _logger.LogInformation($"Scraping URL: {url}, Text XPath: {textXPath}, Image XPath: {imageXPath}");

                // Scrape text
                try
                {
                    var textNodes = doc.DocumentNode.SelectNodes(textXPath) ?? Enumerable.Empty<HtmlNode>();
                    foreach (var node in textNodes)
                    {
                        var text = node.InnerText.Trim();
                        if (!string.IsNullOrEmpty(text) && text.Length > 5)
                        {
                            data.Add(new ScrapedItem(Content: text, ImageUrl: null, ScrapedAt: DateTime.UtcNow));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Invalid text selector '{selector}' (XPath: {textXPath}): {ex.Message}");
                }

                // Scrape images
                try
                {
                    var imageNodes = doc.DocumentNode.SelectNodes(imageXPath) ?? Enumerable.Empty<HtmlNode>();
                    foreach (var node in imageNodes)
                    {
                        var src = node.GetAttributeValue("src", "");
                        if (!string.IsNullOrEmpty(src))
                        {
                            var fullUrl = src.StartsWith("http") ? src : new Uri(new Uri(url), src).ToString();
                            data.Add(new ScrapedItem(Content: null, ImageUrl: fullUrl, ScrapedAt: DateTime.UtcNow));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Invalid image selector '{imageSelector}' (XPath: {imageXPath}): {ex.Message}");
                }

                return data.DistinctBy(d => d.Content ?? d.ImageUrl).Take(100).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to scrape URL {url}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetSuggestedSelectorsAsync(string url)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var selectors = new HashSet<string>();

                var textTags = new[] { "h1", "h2", "h3", "p", "a", ".title", ".content", ".description", ".text", ".author", ".tag" };
                foreach (var tag in textTags)
                {
                    var xPath = CssToXPath(tag);
                    if (doc.DocumentNode.SelectNodes(xPath) != null)
                    {
                        selectors.Add(tag);
                    }
                }

                var imageXPath = CssToXPath("img[src]");
                if (doc.DocumentNode.SelectNodes(imageXPath) != null)
                {
                    selectors.Add("img");
                }

                _logger.LogInformation($"Suggested selectors for {url}: {string.Join(", ", selectors)}");
                return selectors.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to suggest selectors for {url}: {ex.Message}");
                return new List<string> { "h1", "h2", "h3", "p", "img" }; // Fallback
            }
        }

        private string CssToXPath(string cssSelector)
        {
            // Basic CSS-to-XPath converter for common cases
            if (string.IsNullOrWhiteSpace(cssSelector))
                return "//*";

            // Handle multiple selectors (e.g., "h1,h2,h3,p")
            if (cssSelector.Contains(","))
            {
                var selectors = cssSelector.Split(',').Select(s => CssToXPath(s.Trim())).ToList();
                return string.Join(" | ", selectors);
            }

            // Handle tag names (e.g., "h1" -> "//h1")
            if (cssSelector.All(c => char.IsLetterOrDigit(c)))
            {
                return $"//{cssSelector}";
            }

            // Handle class selectors (e.g., ".title" -> "//*[contains(@class, 'title')]")
            if (cssSelector.StartsWith("."))
            {
                var className = cssSelector.Substring(1);
                return $"//*[contains(@class, '{className}')]";
            }

            // Handle attribute selectors (e.g., "img[src]" -> "//img[@src]")
            if (cssSelector.Contains("["))
            {
                var parts = cssSelector.Split('[');
                var tag = parts[0];
                var attr = parts[1].TrimEnd(']');
                return $"//{tag}[@{attr}]";
            }

            // Handle descendant selectors (e.g., ".quote .text" -> "//*[contains(@class, 'quote')]//*[contains(@class, 'text')]")
            if (cssSelector.Contains(" "))
            {
                var parts = cssSelector.Split(' ').Select(s => CssToXPath(s.Trim())).ToList();
                return string.Join("//", parts);
            }

            // Fallback for unsupported selectors
            _logger.LogWarning($"Unsupported CSS selector: {cssSelector}. Using fallback XPath.");
            return $"//*[contains(@class, '{cssSelector.TrimStart('.')}')]";
        }
    }
}