using ShuffleApp.Core.Model;
using Spotify.API.NetCore;
using Spotify.API.NetCore.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShuffleApp.Spotify
{
    public class SpotifyHandler : IMusicService
    {
        SpotifyWebAPI _spotify;
        public void Init(string accessToken)
        {
            _spotify = new()
            {
                TokenType = "Bearer",
                UseAuth = true,
                AccessToken = accessToken
            };
        }
        public async Task<UserProfile> GetUserProfile()
        {
            var response = _spotify.GetPrivateProfile();
            return new UserProfile
            {
                Country = response.Country
            };
        }

        public async Task<IEnumerable<Artist>> GetTopArtistForCurrentUser()
        {
            var response = _spotify.GetUsersTopArtists(TimeRangeType.LongTerm, limit: 50);
            return response.Items
                .Select(p => new Artist
                {
                    Name = p.Name,
                    Popularity = p.Popularity,
                    Id = p.Id
                });
        }

        public async Task<IEnumerable<Device>> GetActiveDeviceForCurrentUser()
        {
            var response = _spotify.GetDevices();


            return response.Devices.Where(p => p.IsActive)
                                    .Select(p => new Device
                                    {
                                        DeviceName = p.Name.ToLower(),
                                        IsActive = p.IsActive,
                                        DeviceId = p.Id
                                    });
        }

        public async Task<IEnumerable<Artist>> SearchArtist(string artistName)
        {
            var response = await _spotify.SearchItemsAsync(artistName, SearchType.Artist, limit: 1);
            return response.Artists.Items.Select(p => new Artist
            {
                Id = p.Id,
                Name = p.Name,
                Popularity = p.Popularity
            });
        }
        public async Task<IEnumerable<Artist>> GetRelatedArtists(string artistId)
        {
            var response = await _spotify.GetRelatedArtistsAsync(artistId);
            return response.Artists.Select(p => new Artist
            {
                Id = p.Id,
                Name = p.Name,
                Popularity = p.Popularity
            });
        }

        public async Task<IEnumerable<Track>> GetTopTracksByArtist(string artistId, string market)
        {
            var response = await _spotify.GetArtistsTopTracksAsync(artistId, market);
            return response.Tracks.Select(p => new Track
            {
                Id = p.Id,
                Name = p.Name,
                Popularity = p.Popularity
            });
        }

        public async Task Play(List<string> trackIds, string deviceId = "")
        {
            _spotify.ResumePlayback(uris: trackIds, deviceId: deviceId);
        }
    }
}
