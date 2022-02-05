using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Planner.Models;

public class SubjectSubmitModel
{
    [DisplayName("Subject Name")]
    public string SubjectName { get; set; }
    public int TeacherId { get; set; }
    public IEnumerable<SelectListItem> TeachersIdsList { get; set; }

    public IEnumerable<int> SelectedStudentsIds { get; set; }
    public IEnumerable<SelectListItem> StudentsIdsList { get; set; }
}