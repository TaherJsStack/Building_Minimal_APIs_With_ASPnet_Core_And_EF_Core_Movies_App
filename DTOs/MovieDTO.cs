namespace Building_MinimalAPIsMoviesApp.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool InTheaters { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Poster { get; set; }
    }
}
