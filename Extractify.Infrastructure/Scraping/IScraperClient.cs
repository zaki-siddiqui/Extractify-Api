using Extractify.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Infrastructure.Scraping
{
    public interface IScraperClient
    {
        //Task<IEnumerable<(string Text, string? ImageUrl)>> ScrapeAsync(string url, string textSelector, string? imageSelector = null);

        Task<List<ScrapedItem>> ScrapeAsync(string url, string? selector = null, string? imageSelector = null);
        Task<List<string>> GetSuggestedSelectorsAsync(string url);
    }
}
