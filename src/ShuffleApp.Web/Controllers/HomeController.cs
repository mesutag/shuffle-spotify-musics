using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShuffleApp.Spotify;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ShuffleApp.Web.Models;
using ShuffleApp.Core.Model;

namespace ShuffleApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMusicService _musicService;

        public HomeController(ILogger<HomeController> logger, IMusicService musicService)
        {
            _logger = logger;
            _musicService = musicService;
        }
        public async Task<IActionResult> Index(IndexViewModel viewModel)
        {
            if (viewModel.TopArtist != null)
            {
                return View(viewModel);
            }
            IndexViewModel model = new();
            if (!User.Identity.IsAuthenticated)
            {
                return View("Index", model);
            }

            string accessToken = await HttpContext.GetTokenAsync("Spotify", "access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Redirect("~/Account/Logout");
            }
            _musicService.Init(accessToken);

            UserProfile user = new();
            try
            {
                await _musicService.GetUserProfile();
            }
            catch (System.Exception)
            {
                return Redirect("~/Account/Logout");
            }


            IEnumerable<Artist> topArtist = await _musicService.GetTopArtistForCurrentUser();
            model.TopArtist = topArtist.Select(p => new ArtistModel
            {
                Name = p.Name.ToLower()
            }).ToList();


            model.SearchQuery = string.Join(", ", topArtist.Take(5).Select(p => p.Name.ToLower()));

            IEnumerable<Device> devices = await _musicService.GetActiveDeviceForCurrentUser();
            model.Devices = devices.Select(p => new DeviceModel
            {
                Name = p.DeviceName.ToLower(),
                IsActive = p.IsActive,
                Id = p.DeviceId
            }).ToList();

            if (model.Devices?.Count > 0)
            {
                model.SelectedDeviceId = model.Devices[0].Id;
            }

            return View("Index", model);
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

            string accessToken = await HttpContext.GetTokenAsync("Spotify", "access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Redirect("~/Account/Logout");
            }


            _musicService.Init(accessToken);



            UserProfile user = await _musicService.GetUserProfile();

            IEnumerable<Artist> topArtist = await _musicService.GetTopArtistForCurrentUser();
            model.TopArtist = topArtist.Select(p => new ArtistModel
            {
                Name = p.Name.ToLower()
            }).ToList();


            IEnumerable<Device> devices = await _musicService.GetActiveDeviceForCurrentUser();
            model.Devices = devices.Select(p => new DeviceModel
            {
                Name = p.DeviceName.ToLower(),
                IsActive = p.IsActive,
                Id = p.DeviceId
            }).ToList();

            if (string.IsNullOrEmpty(searchquery))
            {
                searchquery = string.Join(", ", topArtist.Take(5).Select(p => p.Name.ToLower()));

            }
            model.SearchQuery = searchquery;



            string[] artistQueryKeys = searchquery.Split(",", System.StringSplitOptions.TrimEntries);

            List<Artist> searhedArtists = new();
            List<Artist> relatedArtists = new();
            foreach (var item in artistQueryKeys.Where(p => !string.IsNullOrEmpty(p)))
            {
                var response = (await _musicService.SearchArtist(item)).FirstOrDefault();
                searhedArtists.Add(response);
                relatedArtists.AddRange(await _musicService.GetRelatedArtists(response.Id));

            }

            model.RelatedArtistsBySearchedArtist = relatedArtists.OrderByDescending(p => p.Popularity)
                                                                .Take(30)
                                                                .Select(p => new ArtistModel
                                                                {
                                                                    Name = p.Name
                                                                }).ToList();

            List<Track> tracks = new();
            foreach (var item in searhedArtists)
            {
                tracks.AddRange(await _musicService.GetTopTracksByArtist(item.Id, user.Country));

            }
            var trackUris = tracks.OrderByDescending(p => p.Popularity)
                                    .Select(p => p.ToUri())
                                    .ToList();
            await _musicService.Play(trackUris, deviceId);




            return View("Index", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
