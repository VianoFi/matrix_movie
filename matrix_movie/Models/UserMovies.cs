using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace matrix_movie.Models
{
    public class UserMovie
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }

        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        [DataType(DataType.Date)]
        public DateTime WatchDate { get; set; } = DateTime.Now;

        [MaxLength(300)]
        public string? Comment { get; set; }
    }
}
