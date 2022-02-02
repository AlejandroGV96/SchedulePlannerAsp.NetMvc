using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule_Planner.Data;
using Schedule_Planner.Models;

namespace Schedule_Planner.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
            if (_db.User.Any()) return;
            _db.User.Add(new UserModel()
            {
                Password = "admin",
                Name = "",
                Role = "administrator",
                UserName = "admin"

            });
            _db.SaveChanges();
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            UserModel? selectUser = null;
            try
            {
                IEnumerable<UserModel> userList = _db.User;
                selectUser = userList
                    .First(user => user.UserName.Equals(username));
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                TempData["ErrorNotFound"] = "Error. User does not exist!";
            }


            // Verify the credentials
            if (selectUser?.Password == password && selectUser is not null)
            {
                var claims = new List<Claim>();
                claims.Add(new Claim("username", selectUser.UserName));
                claims.Add(new Claim("Role", selectUser.Role));
                claims.Add(new Claim("Id", selectUser.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, selectUser.Name));
                claims.Add(new Claim(ClaimTypes.Role, selectUser.Role));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                
                return Redirect("/Home/Index");
            }

            TempData["ErrorIncorrect"] = "Error. Password is incorrect";
            return View("Login");
        }
        
        [Authorize]
        [DisplayName("My Account")]
        public IActionResult Account()
        {
            IEnumerable<UserModel> userList = _db.User;
            var selectUser = userList
                .First(user => user.Id.ToString() == User.FindFirst("Id").Value);
            return View(selectUser);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}

