using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotify.API.NetCore.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public async Task<ActionResult> Play(string searchquery, string selectedDevice)
        {
            IndexViewModel model = new();
            string deviceId = selectedDevice;
            model.SelectedDeviceId = selectedDevice;
            if (!User.Identity.IsAuthenticated)
            {
                return View("Index", model);
            }

            Spotify.API.NetCore.SpotifyWebAPI _spotify = new()
            {
                TokenType = "Bearer",
                UseAuth = true
            };
            string accessToken = await HttpContext.GetTokenAsync("Spotify", "access_token");
            _spotify.AccessToken = accessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                return Redirect("~/Account/Logout");
            }

            var user = _spotify.GetPrivateProfile();


            var topArtist = _spotify.GetUsersTopArtists(Spotify.API.NetCore.Enums.TimeRangeType.LongTerm, limit: 50);
            model.TopArtist = topArtist.Items.Select(p => new ArtistViewModel
            {
                Name = p.Name.ToLower()
            }).ToList();


            var devices = _spotify.GetDevices();


            model.Devices = devices.Devices.Select(p => new DeviceViewModel
            {
                Name = p.Name.ToLower(),
                IsActive = p.IsActive,
                Id = p.Id
            }).ToList();

            if (string.IsNullOrEmpty(searchquery))
            {
                model.SearchQuery = string.Join(", ", topArtist.Items.Take(5).Select(p => p.Name.ToLower()));

                return View("Index", model);
            }
            model.SearchQuery = searchquery;



            string[] artists = searchquery.Split(",");

            List<FullArtist> fullArtists = new List<FullArtist>();
            List<FullArtist> relatedFullArtists = new List<FullArtist>();
            foreach (var item in artists)
            {
                var artist = _spotify.SearchItems(item, Spotify.API.NetCore.Enums.SearchType.Artist, limit: 1).Artists.Items.FirstOrDefault();
                fullArtists.Add(artist);
                relatedFullArtists.AddRange(_spotify.GetRelatedArtists(artist.Id).Artists);

            }

            model.RelatedArtistsBySearchedArtist = relatedFullArtists.OrderByDescending(p => p.Popularity).Take(30).Select(p => new ArtistViewModel
            {
                Name = p.Name
            }).ToList();

            List<FullTrack> tracks = new List<FullTrack>();
            foreach (var item in fullArtists)
            {
                tracks.AddRange(_spotify.GetArtistsTopTracks(item.Id, user.Country).Tracks);

            }



            _spotify.ResumePlayback(uris: tracks.OrderByDescending(p => p.Popularity).Select(p => p.Uri).ToList(), deviceId: deviceId);




            return View("Index", model);
        }
        public async Task<IActionResult> Index(IndexViewModel viewModel)
        {
            if (viewModel.TopArtist != null)
            {
                return View(viewModel);
            }
            IndexViewModel model = new IndexViewModel();
            if (!User.Identity.IsAuthenticated)
            {
                return View("Index", model);
            }

            Spotify.API.NetCore.SpotifyWebAPI _spotify = new Spotify.API.NetCore.SpotifyWebAPI();
            _spotify.TokenType = "Bearer";
            _spotify.UseAuth = true;
            string accessToken = await HttpContext.GetTokenAsync("Spotify", "access_token");
            _spotify.AccessToken = accessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                return Redirect("~/Account/Logout");
            }

            PrivateProfile user = new PrivateProfile();
            try
            {
                _spotify.GetPrivateProfile();
            }
            catch (System.Exception)
            {
                return Redirect("~/Account/Logout");
            }


            var topArtist = _spotify.GetUsersTopArtists(Spotify.API.NetCore.Enums.TimeRangeType.LongTerm, limit: 50);
            model.TopArtist = topArtist.Items.Select(p => new ArtistViewModel
            {
                Name = p.Name.ToLower()
            }).ToList();


            var devices = _spotify.GetDevices();

            model.Devices = devices.Devices.Select(p => new DeviceViewModel
            {
                Name = p.Name.ToLower(),
                IsActive = p.IsActive,
                Id = p.Id
            }).ToList();

            if (model.Devices?.Count > 0)
            {
                model.SelectedDeviceId = model.Devices[0].Id;
            }

            return View("Index", model);
        }

        public async Task<IActionResult> Privacy(string searchquery)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
