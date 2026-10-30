using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMovieCRUD.Models
{
    public class Director
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        // Navigation property: One Director has Many Movies
        // public List<Movie> Movies { get; set; } = [];
        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    }
}