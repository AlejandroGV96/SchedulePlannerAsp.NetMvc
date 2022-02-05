using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Schedule_Planner.Data;
using Schedule_Planner.Models;

namespace Schedule_Planner.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubjectController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [Authorize]
        public IActionResult Index()
        {
            List<SubjectModel?>? subjectList = null;
            switch (User.FindFirst("Role").Value)
            {
                case "administrator":
                    subjectList = _db.Subject
                        .GroupBy(subject => new{subject.TeacherId, subject.SubjectName})
                        .Select(subjectGroup => subjectGroup.FirstOrDefault())
                        .ToList();
                    subjectList.Sort((x,y) => string.Compare(x.SubjectName,y.SubjectName));

                    break;
                case "teacher":
                    subjectList = _db.Subject
                        .Where( subject => subject.TeacherId.ToString() == User.FindFirst("Id").Value )
                        .GroupBy(subject => new{subject.TeacherId, subject.SubjectName})
                        .Select(subjectGroup => subjectGroup.FirstOrDefault())
                        .ToList();
                    subjectList.Sort((x,y) => string.Compare(x.SubjectName,y.SubjectName));
                    break;
                case "student":
                    subjectList = _db.Subject
                        .Where( subject => subject.StudentId.ToString() == User.FindFirst("Id").Value )
                        .GroupBy(subject => new{subject.TeacherId, subject.SubjectName})
                        .Select(subjectGroup => subjectGroup.FirstOrDefault())
                        .ToList();
                    subjectList.Sort((x,y) => string.Compare(x.SubjectName,y.SubjectName));
                    break;
            }
           
            return View(subjectList);
        }
        
        // GET - Create
        [Authorize(Roles = "administrator")]
        public IActionResult Create()
        {

            SubjectSubmitModel model = new SubjectSubmitModel();
            model.StudentsIdsList = _db.User.Where(x=> x.Role=="student")
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            model.TeachersIdsList = _db.User.Where(x=> x.Role== "teacher")
                .Select(x=> new SelectListItem
                {
                   Value = x.Id.ToString(),
                   Text = x.Name
                }).OrderBy(x => x.Text);
            return View(model);
        }
        
        // POST - Create
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubjectSubmitModel subject)
        {
            //if (!ModelState.IsValid) return View(subject);
            if(_db.User.Find(subject.TeacherId) is null) return View(subject);
            
            for (var i = 0; i < subject.SelectedStudentsIds.Count(); i++)
            {
                _db.Subject.Add(new SubjectModel
                {
                    
                    SubjectName = subject.SubjectName,
                    StudentId = subject.SelectedStudentsIds.ElementAt(i),
                    TeacherId = _db.User.Find(subject.TeacherId).Id,
                    TeacherName = _db.User.Find(subject.TeacherId).Name
                });
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET ONLY - Details
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }
            var obj = _db.Subject.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            var subjectDetails = _db.Subject.Find(id);
            var subjects = _db.Subject
                .Where(subj => subj.TeacherId == subjectDetails.TeacherId)
                .Where(subj => subj.SubjectName == subjectDetails.SubjectName)
                .ToList();
            var studentsList = new List<UserModel>();
            foreach (var subjRecord in subjects.Where(subject => _db.User.Find(subject.StudentId) is not null))
            {
                studentsList.Add(_db.User.Find(subjRecord.StudentId));
            }

            var Details = new Tuple<List<UserModel>, SubjectModel>(studentsList, subjectDetails);
            return View(Details);
        }
        
        // GET - Delete
        [Authorize(Roles = "administrator")]
        public IActionResult Delete(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }
            var obj = _db.Subject.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        
        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        public IActionResult DeletePost(int? id)
        {
            
            var subjectDetails = _db.Subject.Find(id);
            var delSubjects = _db.Subject
                .Where(subj => subj.TeacherId == subjectDetails.TeacherId)
                .Where(subj => subj.SubjectName == subjectDetails.SubjectName)
                .ToList();
            foreach (var student in delSubjects.Where(student => _db.User.Find(student.StudentId) is not null))
            {
                _db.Remove(student);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}