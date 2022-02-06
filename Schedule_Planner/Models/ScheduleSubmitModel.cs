using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Schedule_Planner.Models;

public class ScheduleSubmitModel
{
    
    
    public DateTime DateTime { get; set; }
    public int TeacherId { get; set; }
    
    [DisplayName("Subject Name")]
    public string SubjectName { get; set; }

}