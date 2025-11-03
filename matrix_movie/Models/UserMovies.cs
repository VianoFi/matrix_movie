using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace matrix_movie.Models
{
    public class UserMovie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId obbligatorio")]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        [Required(ErrorMessage = "MovieId obbligatorio")]
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }

        public Movie? Movie { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime WatchDate { get; set; } = DateTime.Now;

        [MaxLength(300, ErrorMessage = "Il commento non può superare 300 caratteri")]
        [RegularExpression(@"^[a-zA-Z0-9àèéìòùÀÈÉÌÒÙ\s\-:',\.!?()\[\]\n\r]*$",
            ErrorMessage = "Il commento contiene caratteri non validi")]
        public string? Comment { get; set; }
    }
}