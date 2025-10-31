namespace matrix_movie.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public bool IsWatched { get; set; } // Per utenti loggati
    }
}