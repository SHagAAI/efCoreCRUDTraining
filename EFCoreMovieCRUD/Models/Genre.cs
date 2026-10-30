using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMovieCRUD.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        /// <summary>
        /// Collection of movies that have this genre
        /// many-to-many relationships
        /// 1 Genre can have multiple movies, right ?
        /// </summary>
         public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    }
}