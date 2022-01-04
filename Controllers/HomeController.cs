using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HobbiesExam.Models;
//extra
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using  Microsoft.AspNetCore.Server.Kestrel.Core;

//extra probably for sessions
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace HobbiesExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }

        //index page has both registration and login
        [HttpGet("")] //basically like combining get actions for 
        public IActionResult Index()
        {
            return View();
        }

        //Post to registration
        [HttpPost("Registration")]
        public IActionResult Registration(User user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided UserName
                if (_context.Users.Any(u => u.UserName == user.UserName))
                {
                    // Manually add a ModelState error to the UserName field, with provided
                    // error message
                    ModelState.AddModelError("UserName", "Username already in use!");

                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // if everything is okay save the user to the DB
                // Initializing a PasswordHasher object, providing our User class as its type
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                User userInDb = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return RedirectToAction("AllHobbies");
            }
            // other code
            return View("Index");
        }

        //Post to login
        [HttpPost("LoginPost")] //try changing to /login
        public IActionResult LoginPost(Login userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided UserName
                var userInDb = _context.Users.FirstOrDefault(u => u.UserName == userSubmission.UserName);
                
                // If no user exists with provided UserName
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("UserName", "Invalid Username/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<Login>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing UserName" is handled)
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View("Login");
                }
                // assign user ID to sessions
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return RedirectToAction("AllHobbies");
            }
            // go back to login if fails
            return View("Index");
        }

        //Get all hobbies view
        [HttpGet("AllHobbies")]
         public IActionResult AllHobbies()
            {
                if (HttpContext.Session.GetInt32("userId") == null)
                {
                    return RedirectToAction("Index");
                }
                //get every hobby and grab its enthusiasts
                List<Hobby> EveryHobby = _context.Hobbies
                .Include(h => h.Enthusiasts)
                .ToList();

                ViewBag.AllHobbies = EveryHobby;
                ViewBag.UserId = (int)HttpContext.Session.GetInt32("userId");
                
                return View("AllHobbies");
            }

        //View selected hobby
        [HttpGet("SingleHobby/{hobbyId}")]
        public IActionResult SingleHobby(int hobbyId)
            {
                // get the selected hobby
                ViewBag.SpecificHobby = _context.Hobbies
                    .FirstOrDefault(w => w.HobbyId == hobbyId);

                // Get all enthusiasts from hobby
                var hobbyEnthusiasts = _context.Hobbies
                .Include(w => w.Enthusiasts)
                .ThenInclude(u => u.user)
                .FirstOrDefault(w => w.HobbyId == hobbyId);
            ViewBag.LoggedInUser = (int)HttpContext.Session.GetInt32("userId");
            
            ViewBag.AllEnthusiasts = hobbyEnthusiasts.Enthusiasts;
            return View("SingleHobby");
            }

        //Show add hobby
        [HttpGet("AddHobby")]
        public IActionResult AddHobby()
            {
                if (HttpContext.Session.GetInt32("userId") == null)
                {
                    return RedirectToAction("Index");
                }
                return View("AddHobby");
            }

        //Post to AddHobby
        [HttpPost("AddHobbyPost")]
        public IActionResult AddHobbyPost(Hobby newHobby)
            {   
               if (ModelState.IsValid)
                {
                    // If hobby already exists
                    if (_context.Hobbies.Any(u => u.Name == newHobby.Name))
                    {
                        // Manually add a ModelState error to the UserName field, with provided
                        // error message
                        ModelState.AddModelError("Name", "Hobby already exists!");

                        // You may consider returning to the View at this point
                        return View("AddHobby");
                    }
                    //newHobby.UserId = (int)HttpContext.Session.GetInt32("userId");
                    _context.Add(newHobby);
                    _context.SaveChanges();
                    Hobby thisHobby = _context.Hobbies.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
                    ViewBag.LoggedInUser = (int)HttpContext.Session.GetInt32("userId");
                    return Redirect("/SingleHobby/"+thisHobby.HobbyId);
                }
                return View("AddHobby", newHobby);
            }
            // Adding a user to a hobby (enthusiast)
            [HttpPost("AddEnthusiast")]
            public IActionResult AddEnthusiast(Association newEnthusiast)
                {
                    if (ModelState.IsValid)
                    {
                        //var testquery = _context.Hobbies.Include(u => u.Enthusiasts).Where(u)
                        // {
                        //     // Manually add a ModelState error to the UserName field, with provided
                        //     // error message
                        //     ModelState.AddModelError("UserId", "You already added this hobby");

                        //     // You may consider returning to the View at this point
                        //     return View("AddHobby");
                        // }
                    _context.Associations.Add(newEnthusiast);
                    _context.SaveChanges();
                    ViewBag.LoggedInUser = (int)HttpContext.Session.GetInt32("userId");
                    return RedirectToAction("AllHobbies");
                    }
                    return RedirectToAction("AllHobbies");
                }
        
        // Form to update hobby
        [HttpGet("update/{hobbyId}")]
        public IActionResult UpdateHobby(int hobbyId)
        {
            Hobby SelectedHobby = _context.Hobbies.FirstOrDefault(hobby => hobby.HobbyId == hobbyId);
            ViewBag.ChosenHobby = SelectedHobby;
            return View();
        }

        // Post action to update hobby 
         [HttpPost("update/{hobbyId}")]
        public IActionResult UpdateHobbyPost(Hobby updatedhobby, int hobbyId)
        {
            Hobby SelectedHobby = _context.Hobbies.FirstOrDefault(hobby => hobby.HobbyId == hobbyId);
            SelectedHobby.Name = updatedhobby.Name;
            SelectedHobby.Description = updatedhobby.Description;
            SelectedHobby.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("AllHobbies");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
