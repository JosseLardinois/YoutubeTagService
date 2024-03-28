using Microsoft.AspNetCore.Mvc;
using YoutubeTagService.Interface;

namespace YoutubeTagService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly ILogger<YoutubeController> _logger;
        private readonly IYoutubeTagRepository _youtubeTagRepository;
        public YoutubeController(ILogger<YoutubeController> logger, IYoutubeTagRepository youtubeTagRepository)
        {
            _youtubeTagRepository = youtubeTagRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/youtube/mostUsedTags")]
        public async Task<IActionResult> GetMostUsedTags(string channelId)
        {
            if (string.IsNullOrWhiteSpace(channelId))
            {
                return BadRequest("Invalid channel ID provided.");
            }

            try
            {
                var mostUsedTags = await _youtubeTagRepository.GetMostUsedTags(channelId);

                if (mostUsedTags.Any())
                {
                    // Serialize and return the entire list of tuples (tags and their counts)
                    return Ok(mostUsedTags);
                }
                else
                {
                    return NotFound("No tags found for the specified channel.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the most used tags.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}