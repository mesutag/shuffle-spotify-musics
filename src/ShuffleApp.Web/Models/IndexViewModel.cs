using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class IndexViewModel
    {
        public List<ArtistViewModel> TopArtist { get; set; }
        public List<DeviceViewModel> Devices { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedDeviceId { get; set; }
        public List<ArtistViewModel> RelatedArtistsBySearchedArtist { get; set; }
    }

    public class ArtistViewModel
    {
        public string Name { get; set; }
    }

    public class DeviceViewModel
    {
        public string Name { get; set; }
        public bool IsActive { get;  set; }
        public string Id { get; set; }
    }
}
