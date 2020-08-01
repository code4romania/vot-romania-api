namespace VotRomania.Services.Location.HereMaps
{
    public class Item
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string ResultType { get; set; }
        public string LocalityType { get; set; }
        public Address Address { get; set; }
        public Position Position { get; set; }
        public Mapview MapView { get; set; }
        public Scoring Scoring { get; set; }
    }
}