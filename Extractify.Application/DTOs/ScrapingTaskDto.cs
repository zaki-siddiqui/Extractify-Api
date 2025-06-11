using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Application.DTOs
{
    public class ScrapingTaskDto
    {
        public int ScrapingTaskId { get; set; }
        [Required, Url]
        public string Url { get; set; } = string.Empty;
        
        public string? Selector { get; set; }
        public string? ImageSelector { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
