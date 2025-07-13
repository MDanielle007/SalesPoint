using System.ComponentModel.DataAnnotations;

namespace SalesPoint.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryCreateDTO
    {
        [Required, StringLength(100)]
        public string Name { get; set; }
    }

    public class CategoryUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
    }
}
