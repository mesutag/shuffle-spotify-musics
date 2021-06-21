using ShuffleApp.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShuffleApp.Spotify
{
    public interface IMusicService
    {
        void Init(string token);
        Task<UserProfile> GetUserProfile();
        Task<IEnumerable<Artist>> GetTopArtistForCurrentUser();
        Task<IEnumerable<Device>> GetActiveDeviceForCurrentUser();
        Task<IEnumerable<Artist>> SearchArtist(string artistName);
        Task<IEnumerable<Artist>> GetRelatedArtists(string artistId);
        Task<IEnumerable<Track>> GetTopTracksByArtist(string artistId, string market);
        Task Play(List<string> trackIds, string deviceId = "");
    }
}