namespace ShuffleApp.Core.Model
{
    public class Track
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }
        public string ToUri()
        {
            return "spotify:track:" + Id;
        }
    }
}
