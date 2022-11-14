using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DTOs
{
    public class EmployeeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Column("blog_id")]
        public string Email { get; set; }

        [Required]
        public DepartmentDto Department { get; set; }
    }
}