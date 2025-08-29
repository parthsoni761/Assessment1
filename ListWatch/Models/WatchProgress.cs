namespace ListWatch.Models
{
    public class WatchProgress
    {
        public int Id { get; set; }

        public int WatchListItemId { get; set; }
        public WatchListItems WatchListItems { get; set; }

        public int CompletedEpisodes { get; set; }
        public int TotalEpisodes { get; set; }
    }
}
