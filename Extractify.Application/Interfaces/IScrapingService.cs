using Extractify.Application.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Application.Interfaces
{
    public interface IScrapingService
    {
        Task<ScrapingTaskDto> CreateTaskAsync(ScrapingTaskDto taskDto);
        Task<ScrapingTaskDto> GetTaskAsync(int id);
        Task<IEnumerable<ScrapingTaskDto>> GetAllTasksAsync();
        Task<bool> ExecuteTaskAsync(int taskId);
        Task<List<string>> GetSuggestedSelectorsAsync(string url);
    }
}
