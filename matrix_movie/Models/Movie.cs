using System.ComponentModel.DataAnnotations;

namespace matrix_movie.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Il titolo deve essere tra 1 e 200 caratteri")]
        [RegularExpression(@"^[a-zA-Z0-9àèéìòùÀÈÉÌÒÙ\s\-:',\.!?()\[\]]+$",
            ErrorMessage = "Il titolo contiene caratteri non validi")]
        public string Title { get; set; }

        [Required(ErrorMessage = "L'URL dell'immagine è obbligatorio")]
        [Url(ErrorMessage = "URL immagine non valido")]
        [StringLength(500, ErrorMessage = "URL troppo lungo")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Il genere è obbligatorio")]
        [StringLength(50, ErrorMessage = "Genere troppo lungo")]
        [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "Il genere può contenere solo lettere")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "L'anno è obbligatorio")]
        [Range(1888, 2100, ErrorMessage = "Anno non valido (deve essere tra 1888 e 2100)")]
        public int Year { get; set; }

        public bool IsWatched { get; set; }

        [StringLength(300, ErrorMessage = "La descrizione non può superare 300 caratteri")]
        [RegularExpression(@"^[a-zA-Z0-9àèéìòùÀÈÉÌÒÙ\s\-:',\.!?()\[\]]*$",
            ErrorMessage = "La descrizione contiene caratteri non validi")]
        public string? Description { get; set; }

        [StringLength(2000, ErrorMessage = "La trama non può superare 2000 caratteri")]
        [RegularExpression(@"^[a-zA-Z0-9àèéìòùÀÈÉÌÒÙ\s\-:',\.!?()\[\]\n\r]*$",
            ErrorMessage = "La trama contiene caratteri non validi")]
        public string? Plot { get; set; }
    }
}