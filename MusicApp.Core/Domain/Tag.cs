using System.ComponentModel.DataAnnotations;

namespace Data.Domain
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
