using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Infrastructure.Models
{
    public record ScrapedItem(string? Content, string? ImageUrl, DateTime ScrapedAt);
}
