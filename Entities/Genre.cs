using System.ComponentModel.DataAnnotations;

namespace Building_Minimal_APIs_With_ASPnet_Core_And_EF_Core_Movies_App.Entities
{
    public class Genre
    {   
        public int Id { get; set; }
        //public string Name { get; set; } = string.Empty;
        //[StringLength(150)]
        public string Name { get; set; } = null!;

    }
}
