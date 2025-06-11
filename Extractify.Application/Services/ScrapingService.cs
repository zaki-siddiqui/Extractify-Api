using AutoMapper;
using Azure.Core;
using Extractify.Application.DTOs;
using Extractify.Application.Interfaces;
using Extractify.Domain.Entities;
using Extractify.Domain.Interfaces;
using Extractify.Infrastructure.Scraping;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extractify.Application.Services
{
    public class ScrapingService : IScrapingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IScraperClient _scraperClient;
        private readonly ILogger<ScrapingService> _logger;

        public ScrapingService(IUnitOfWork unitOfWork, IMapper mapper, IScraperClient scraperClient, ILogger<ScrapingService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _scraperClient = scraperClient;
            _logger = logger;
        }

        public async Task<ScrapingTaskDto> CreateTaskAsync(ScrapingTaskDto request)
        {
            var task = _mapper.Map<ScrapingTask>(request);
            await _unitOfWork.GetRepository<ScrapingTask>().AddAsync(task);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ScrapingTaskDto>(task);
        }

        public async Task<ScrapingTaskDto> GetTaskAsync(int id)
        {
            var task = await _unitOfWork.GetRepository<ScrapingTask>().GetByIdAsync(id);

            return task == null ? null : _mapper.Map<ScrapingTaskDto>(task);
        }

        public async Task<IEnumerable<ScrapingTaskDto>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _unitOfWork.GetRepository<ScrapingTask>().GetAllAsync();
                return _mapper.Map<IEnumerable<ScrapingTaskDto>>(tasks);
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        public async Task<bool> ExecuteTaskAsync(int id)
        {
            var task = await _unitOfWork.GetRepository<ScrapingTask>().GetByIdAsync(id);
            if (task == null) return false;

            try
            {
                var scrapedItems = await _scraperClient.ScrapeAsync(task.Url, task.Selector, task.ImageSelector);
                foreach (var item in scrapedItems)
                {
                    var scrapedData = new ScrapedData
                    {
                        ScrapingTaskId = id,
                        Content = item.Content,
                        ImageUrl = item.ImageUrl,
                        ScrapedAt = item.ScrapedAt
                    };
                    await _unitOfWork.GetRepository<ScrapedData>().AddAsync(scrapedData);
                }

                task.Status = "Completed";
                await _unitOfWork.GetRepository<ScrapingTask>().UpdateAsync(task);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<List<string>> GetSuggestedSelectorsAsync(string url)
        {
            return await _scraperClient.GetSuggestedSelectorsAsync(url);
        }

        //public async Task<bool> ExecuteTaskAsync(int id)
        //{
        //    var task = await _unitOfWork.GetRepository<ScrapingTask>().GetByIdAsync(id);
        //    if (task == null || task.Status == "Running" || task.Status == "Completed")
        //    {
        //        _logger.LogWarning($"Cannot execute task {id}: Invalid state or task not found.");
        //        return false;
        //    }

        //    task.Status = "Running";
        //    await _unitOfWork.SaveChangesAsync();

        //    try
        //    {
        //        _logger.LogInformation($"Executing task {id} for URL {task.Url} with selector {task.Selector}");
        //        var scrapedData = await _scraperClient.ScrapeAsync(task.Url, task.Selector, task.ImageSelector);
        //        foreach (var (text, imageUrl) in scrapedData)
        //        {
        //            await _unitOfWork.GetRepository<ScrapedData>().AddAsync(new ScrapedData
        //            {
        //                ScrapingTaskId = id,
        //                Content = text,
        //                ImageUrl = imageUrl,
        //                ScrapedAt = DateTime.UtcNow
        //            });
        //        }
        //        task.Status = "Completed";
        //        task.CompletedAt = DateTime.UtcNow;
        //        _logger.LogInformation($"Task {id} completed successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        task.Status = "Failed";
        //        _logger.LogError(ex, $"Failed to execute task {id}: {ex.Message}");
        //        throw;
        //    }
        //    finally
        //    {
        //        await _unitOfWork.SaveChangesAsync();
        //    }

        //    return true;
        //}

        //public async Task ExecuteTaskAsync(int taskId)
        //{
        //    var repository = _unitOfWork.GetRepository<ScrapingTask>();
        //    var task = await repository.GetByIdAsync(taskId);
        //    if (task != null)
        //        throw new Exception("Task not found");

        //    task.Status = "Running";
        //    await _unitOfWork.SaveChangesAsync();

        //    try
        //    {
        //        var content = await _scraperClient.ScrapeAsync(task.Url, task.Selector);
        //        var scrapedData = new ScrapedData
        //        {
        //            ScrapingTaskId = taskId,
        //            Content = content,
        //            ScrapedAt = DateTime.UtcNow
        //        };

        //        var dataRepository = _unitOfWork.GetRepository<ScrapedData>();
        //        await dataRepository.AddAsync(scrapedData);

        //        task.Status = "Completed";
        //        task.CompletedAt = DateTime.UtcNow;
        //        await _unitOfWork.SaveChangesAsync();
        //    }
        //    catch(Exception ex)
        //    {
        //        task.Status = "Failed";
        //        await _unitOfWork.SaveChangesAsync();
        //        throw;
        //    }

        //}
    }
}
