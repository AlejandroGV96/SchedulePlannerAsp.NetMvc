using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Schedule_Planner.Models;

public class UserModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}