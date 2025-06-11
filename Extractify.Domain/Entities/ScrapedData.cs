using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Domain.Entities
{
    public class ScrapedData
    {
        [Key]
        public int ScrapedDataId { get; set; }
        public int ScrapingTaskId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl {  get; set; }
        public DateTime ScrapedAt { get; set; }
        public ScrapingTask ScrapingTask { get; set; }
    }
}
