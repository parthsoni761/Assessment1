namespace ListWatch.DTOs
{
    public class DashboardDto
    {
        public int TotalItems { get; set; }
        public int CompletedItems { get; set; }
        public int PendingItems { get; set; }
        public int FavoriteItems { get; set; }   
        public string FavoriteGenre { get; set; } = string.Empty;
        public List<WatchListItemDto> TopRatedItems { get; set; } = new();
    }
}
