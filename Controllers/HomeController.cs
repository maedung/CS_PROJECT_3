using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BeltTest.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("signin")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost("newuser")]
        public IActionResult Register(User newuser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u =>u.email == newuser.email))
                {
                    ModelState.AddModelError("email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newuser.password = Hasher.HashPassword(newuser, newuser.password);
                dbContext.Users.Add(newuser);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
                
            }
            return View("Index");
        }
        [HttpGet("home")]
        public IActionResult HomePage()
        {
            if(HttpContext.Session.GetInt32("userid") == null)
            {
                return View("Index");
            }
            var person = dbContext.Users.Include(user => user.Posts).ThenInclude(post => post.Activity).FirstOrDefault(user => user.id == HttpContext.Session.GetInt32("userid"));
            List<Models.Activity> Allactivities = dbContext.Activities.Include(activity => activity.Creator).Include(activity => activity.PostedActivities).ThenInclude(a => a.User).ToList();
            ViewBag.Allactivities = Allactivities;
            foreach(var i in Allactivities)
            {
                if(i.date.Date <= DateTime.Now.Date)
                {
                    if(i.time.TimeOfDay <= DateTime.Now.TimeOfDay)
                    {
                        dbContext.Activities.Remove(i);
                        dbContext.SaveChanges();
                    }
                }
            }

            return View(person);
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.email == user.login_email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("login_email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.password, user.login_password);
                if(result == 0)
                {
                    ModelState.AddModelError("login_email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("userid", userInDb.id);
                return Redirect("/home");
            }
            return View("Index");
        }
        [HttpGet("new")]
        public IActionResult NewActivity()
        {
            return View();
        }
        [HttpPost("AddActivity")]
        public IActionResult AddActivity(Models.Activity newactivity, string duration_type)
        {
            if(ModelState.IsValid)
            {
                if(newactivity.date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("date", "Activity must be in the future");
                    return View("NewActivity");
                }
                if(newactivity.date == DateTime.Now.Date && newactivity.time <= DateTime.Now)
                {
                    ModelState.AddModelError("time", "Activity must be in the future");
                    return View("NewActivity");
                }
                
                newactivity.duration = $"{newactivity.duration} " + $"{duration_type}";
                var thisuser = dbContext.Users.FirstOrDefault(u => u.id == HttpContext.Session.GetInt32("userid"));
                newactivity.Creator = thisuser;
                dbContext.Activities.Add(newactivity);
                dbContext.SaveChanges();
                return Redirect($"/activity/{newactivity.id}");
            }
            return View("NewActivity");
        }
        [HttpGet("activity/{id}")]
        public IActionResult ActivityInfo(int id)
        {
            var activity = dbContext.Activities.Include(a => a.Creator).Include(a => a.PostedActivities).ThenInclude(a => a.User).FirstOrDefault(u => u.id == id);
            ViewBag.user = HttpContext.Session.GetInt32("userid");
            return View(activity);
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var thisactivity = dbContext.Activities.FirstOrDefault(u => u.id == id);
            dbContext.Activities.Remove(thisactivity);
            dbContext.SaveChanges();
            return RedirectToAction("HomePage");
        }

        [HttpGet("join/{activityid}/{userid}")]
        public IActionResult Join(int activityid, int userid)
        {
            var newpost = new Post();
            newpost.activityid = activityid;
            newpost.userid = userid;
            dbContext.Posts.Add(newpost);
            dbContext.SaveChanges();
            return RedirectToAction("HomePage");
        }

        [HttpGet("leave/{activityid}/{userid}")]
        public IActionResult Leave(int activityid, int userid)
        {
            Post post = dbContext.Posts.Where(u => u.activityid == activityid).SingleOrDefault(u => u.userid == userid);
            dbContext.Posts.Remove(post);
            dbContext.SaveChanges();
            return RedirectToAction("HomePage");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
