using Extractify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<ScrapingTask> ScrapingTasks { get; set; }
        public DbSet<ScrapedData> ScrapedData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScrapingTask>()
            .HasMany(t => t.ScrapedData)
            .WithOne(d => d.ScrapingTask)
            .HasForeignKey(d => d.ScrapingTaskId);
        }
    }
}
