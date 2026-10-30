using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMovieCRUD.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }
        public int ReleaseYear { get; set; }

        /// <summary>
        /// Collection of genre this movie have
        /// many-to-many relationships
        /// 1 movie can have multiple genres, right ?
        /// </summary>
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

        public int DirectorId { get; set; }
        public Director Director { get; set; } = null!;


    }
}