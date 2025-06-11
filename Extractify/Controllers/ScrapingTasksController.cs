using AutoMapper;
using Extractify.Application.DTOs;
using Extractify.Application.Interfaces;
using Extractify.Domain.Entities;
using Extractify.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Extractify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapingTasksController : ControllerBase
    {
        private readonly IScrapingService _scrapingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScrapingTasksController(IScrapingService scrapingService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _scrapingService = scrapingService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] ScrapingTaskDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var task = await _scrapingService.CreateTaskAsync(request);
                return CreatedAtAction(nameof(GetTask), new { id = task.ScrapingTaskId }, task);
            }
            catch(AutoMapperMappingException ex)
            {
                return BadRequest(new { Message = "Invalid task data.", Detail = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Error creating task.", Detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var task = await _scrapingService.GetTaskAsync(id);
                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving task.", Detail = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _scrapingService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { Message = "Error retrieving tasks.", Detail = ex.Message });
            }
            
        }

        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecuteTask(int id)
        {
            try
            {
                var success = await _scrapingService.ExecuteTaskAsync(id);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Error executing task.", Detail = ex.Message });
            }
        }


        [HttpGet("{id}/data")]
        public async Task<ActionResult<IEnumerable<ScrapedDataDto>>> GetScrapedData(int id)
        {
            var task = await _scrapingService.GetTaskAsync(id);
            if (task == null) return NotFound();

            //var data = await _unitOfWork.GetRepository<ScrapedData>().GetAllAsync();
            var data = await _unitOfWork.GetRepository<ScrapedData>().GetAsync(d => d.ScrapingTaskId == id);
            var dataDtos = _mapper.Map<IEnumerable<ScrapedDataDto>>(data);
            return Ok(dataDtos);
        }

        [HttpGet("suggest-selectors")]
        public async Task<ActionResult<IEnumerable<string>>> GetSuggestedSelectors([FromQuery] string url)
        {
            var selectors = await _scrapingService.GetSuggestedSelectorsAsync(url);
            return Ok(selectors);
        }

    }
}
