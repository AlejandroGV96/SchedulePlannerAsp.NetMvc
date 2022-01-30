using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Schedule_Planner.Models;

public class CredentialModel
{
    [Required]
    [DisplayName("User name")]
    public string UserName { get; set; }
    [Required]
    [DisplayName("Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}