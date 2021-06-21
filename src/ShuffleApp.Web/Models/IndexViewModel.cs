using System.Collections.Generic;

namespace ShuffleApp.Web.Models
{
    public class IndexViewModel
    {
        public List<ArtistModel> TopArtist { get; set; }
        public List<DeviceModel> Devices { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedDeviceId { get; set; }
        public List<ArtistModel> RelatedArtistsBySearchedArtist { get; set; }
    }
}
