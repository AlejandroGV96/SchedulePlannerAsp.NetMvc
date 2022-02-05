using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Planner.Models;

[Index(nameof(SubjectName),nameof(StudentId), IsUnique = true)]
public class SubjectModel
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("UserModel")]
    public int StudentId { get; set; }
    [ForeignKey("UserModel")]
    public int TeacherId { get; set; }
    
    [Required]
    [DisplayName("Subject")]
    public string SubjectName { get; set; }
    [Required]
    [DisplayName("Teacher")]
    public string TeacherName { get; set; }

}