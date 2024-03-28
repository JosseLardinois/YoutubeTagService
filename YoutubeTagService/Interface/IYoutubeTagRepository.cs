using System.Security.Principal;
using YoutubeTagService.DTO;

namespace YoutubeTagService.Interface
{
    public interface IYoutubeTagRepository
    {
        Task<List<TagCount>> GetMostUsedTags(string channelId);
    }
}
