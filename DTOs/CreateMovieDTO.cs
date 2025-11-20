namespace Building_MinimalAPIsMoviesApp.DTOs
{
    public class CreateMovieDTO
    {
        public string Title { get; set; } = string.Empty;
        public bool InTheaters { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
