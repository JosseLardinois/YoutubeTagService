using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Security.Principal;
using YoutubeTagService.DTO;
using YoutubeTagService.Interface;

namespace YoutubeTagService.BLL
{
    public class YoutubeTagRepository : IYoutubeTagRepository
    {
        private readonly YouTubeService _youtubeService;
        private readonly ILogger<YoutubeTagRepository> _logger;
        public YoutubeTagRepository(ILogger<YoutubeTagRepository> logger)
        {

            _logger = logger;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Environment.GetEnvironmentVariable("YoutubeAPIKey"),
                ApplicationName = this.GetType().ToString()
            });
        }

        public async Task<List<TagCount>> GetMostUsedTags(string channelId)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.ChannelId = channelId;
            searchListRequest.MaxResults = 50;
            searchListRequest.Type = "video";

            var searchListResponse = await searchListRequest.ExecuteAsync();
            var videoIds = searchListResponse.Items
                                              .Where(item => item.Id.Kind == "youtube#video")
                                              .Select(item => item.Id.VideoId)
                                              .ToList();

            var tags = new List<string>();
            if (videoIds.Count > 0)
            {
                var videoRequest = _youtubeService.Videos.List("snippet");
                videoRequest.Id = string.Join(",", videoIds);
                var videoResponse = await videoRequest.ExecuteAsync();

                foreach (var video in videoResponse.Items)
                {
                    if (video.Snippet.Tags != null)
                    {
                        tags.AddRange(video.Snippet.Tags);
                    }
                }
            }

            return tags.GroupBy(t => t)
                       .OrderByDescending(g => g.Count())
                       .Select(g => new TagCount { Tag = g.Key, Count = g.Count() })
                       .ToList();
        }

    }
}
