using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Application.DTOs
{
    public class ScrapedDataDto
    {
        public int Id { get; set; }
        public int ScrapingTaskId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime ScrapedAt { get; set; }
    }
}
