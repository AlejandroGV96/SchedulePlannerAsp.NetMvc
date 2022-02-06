using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Schedule_Planner.Data;
using Schedule_Planner.Models;

namespace Schedule_Planner.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ScheduleController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [Authorize]
        public IActionResult Index()
        {
            List<ScheduleModel?>? scheduleList = null;
            switch (User.FindFirst("Role").Value)
            {
                case "administrator":
                    scheduleList = _db.Schedule
                        .GroupBy(schedule => new{schedule.TeacherId, schedule.SubjectName, schedule.DateTime})
                        .Select(subjectGroup => subjectGroup.FirstOrDefault())
                        .ToList();
                    scheduleList.Sort((x,y) => DateTime.Compare(x.DateTime,y.DateTime));

                    break;
                case "teacher":
                    scheduleList = _db.Schedule
                        .Where( schedule => schedule.TeacherId.ToString() == User.FindFirst("Id").Value )
                        .GroupBy(schedule => new{schedule.DateTime, schedule.SubjectName})
                        .Select(subjectGroup => subjectGroup.FirstOrDefault())
                        .ToList();
                    scheduleList.Sort((x,y) => DateTime.Compare(x.DateTime,y.DateTime));

                    var subjects = _db.Subject
                        .Where(subjects => subjects.TeacherId.ToString() == User.FindFirst("Id").Value)
                        .GroupBy(subjects => subjects.SubjectName)
                        .Select(g => g.First().SubjectName)
                        .ToList();
                    ViewData["subjects"] = subjects;
                    break;
                case "student":
                    scheduleList = _db.Schedule
                        .Where( schedule => schedule.StudentId.ToString() == User.FindFirst("Id").Value )
                        .Select(subjectGroup => subjectGroup)
                        .ToList();
                    scheduleList.Sort((x,y) => DateTime.Compare(x.DateTime,y.DateTime));
                    break;
            }
           
            return View(scheduleList);
        }
        
        // GET - Create
        [Authorize(Roles = "teacher")]
        public IActionResult Create(string? id)
        {
            string[] split = id.Split(',');
            var teacherId = split[0];
            var subject = split[1];
            ScheduleSubmitModel model = new ScheduleSubmitModel();

            model.TeacherId = int.Parse(teacherId);
            model.SubjectName = subject;
            model.DateTime = DateTime.Now;
            return View(model);
        }
        
        // POST - Create
        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ScheduleSubmitModel schedule)
        {   
            if(_db.User.Find(schedule.TeacherId) is null) return View(schedule);
            var subjectRecords = _db.Subject
                .Where(record => record.TeacherId == schedule.TeacherId)
                .Where(record => record.SubjectName == schedule.SubjectName);
            var studentsInClass = new List<int>();
            foreach (var record in subjectRecords)
            {
                studentsInClass.Add(record.StudentId);
            }
            for (var i = 0; i < studentsInClass.Count; i++)
            {
                _db.Schedule.Add(new ScheduleModel
                {
                    SubjectName = schedule.SubjectName,
                    StudentId = studentsInClass.ElementAt(i),
                    TeacherId = schedule.TeacherId,
                    TeacherName = _db.User.Find(schedule.TeacherId).Name,
                    DateTime = schedule.DateTime
                });
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        

        // GET - Delete
        [Authorize(Roles = "teacher,administrator")]
        public IActionResult Delete(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }
            var obj = _db.Schedule.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        
        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "teacher,administrator")]
        public IActionResult DeletePost(int? id)
        {
            
            var scheduleDetails = _db.Schedule.Find(id);
            var delSchedule = _db.Schedule
                .Where(sched => sched.TeacherId == scheduleDetails.TeacherId)
                .Where(sched => sched.SubjectName == scheduleDetails.SubjectName)
                .Where(sched => sched.DateTime == scheduleDetails.DateTime)
                .ToList();
            foreach (var student in delSchedule.Where(student => _db.User.Find(student.StudentId) is not null))
            {
                _db.Remove(student);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
    
}