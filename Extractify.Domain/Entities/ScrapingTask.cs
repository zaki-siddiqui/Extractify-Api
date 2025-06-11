using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Domain.Entities
{
    public class ScrapingTask
    {
        [Key]
        public int ScrapingTaskId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Selector { get; set; }
        public string? ImageSelector { get; set; }
        public string Status { get; set; } = "Pending"; // e.g., Pending, Running, Completed, Failed
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<ScrapedData> ScrapedData { get; set; } = new List<ScrapedData>();
    }
}
