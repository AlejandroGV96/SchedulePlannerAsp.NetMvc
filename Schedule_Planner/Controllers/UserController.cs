using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Planner.Data;
using Schedule_Planner.Models;

namespace Schedule_Planner.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [Authorize(Roles = "administrator")]
        public IActionResult Index()
        {
            //IEnumerable<UserModel> objList = _db.User.OrderBy( s => s.Name);
            List<UserModel> objList = _db.User.ToList();
            objList.Sort( (x, y) => string.Compare(x.Name,y.Name));
            return View(objList);
        }
        
        // GET - Create
        [Authorize(Roles = "administrator")]
        public IActionResult Create()
        {
            return View();
        }
        
        // POST - Create
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                _db.User.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
        
        // GET - Update
        [Authorize(Roles = "administrator")]
        public IActionResult Update(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }
            var obj = _db.User.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        
        // POST - Update
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UserModel obj)
        {
            if (!ModelState.IsValid) return View(obj);
        
            _db.User.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET - Delete
        public IActionResult Delete(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }
            var obj = _db.User.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
    
        // POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.User.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            var SchedulesToDelete = _db.Schedule
                .Where(schedule => schedule.StudentId == id);
            var SchedulesToDeleteTeacher = _db.Schedule
                .Where(schedule => schedule.TeacherId == id);
            var SubjectToDelete = _db.Subject
                .Where(subject => subject.StudentId == id);
            var SubjectToDeleteTeacher = _db.Subject
                .Where(subject => subject.TeacherId == id);
            
            foreach (var schedule in SchedulesToDelete )
            {
                _db.Schedule.Remove(schedule);
            }
            
            foreach (var scheduleT in SchedulesToDeleteTeacher )
            {
                _db.Schedule.Remove(scheduleT);
            }
            
            foreach (var subject in SubjectToDelete )
            {
                _db.Subject.Remove(subject);
            }
            
            foreach (var subjectT in SubjectToDeleteTeacher )
            {
                _db.Subject.Remove(subjectT);
            }
            
            _db.User.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}