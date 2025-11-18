using System.ComponentModel.DataAnnotations;

namespace Building_MinimalAPIsMoviesApp.Entities
{
    public class Genre
    {   
        public int Id { get; set; }
        //public string Name { get; set; } = string.Empty;
        //[StringLength(150)]
        public string Name { get; set; } = null!;

    }
}
