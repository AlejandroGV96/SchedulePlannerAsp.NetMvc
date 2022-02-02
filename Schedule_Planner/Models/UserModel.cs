using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Planner.Models;
[Index(nameof(UserName), IsUnique = true)]
public class UserModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    [DisplayName("User Name")]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
    public string Name { get; set; }

}