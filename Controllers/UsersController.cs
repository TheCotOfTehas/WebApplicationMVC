using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WebApplicationMVC.EntitiFramework;
using WebApplicationMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC.Controllers
{
    public class UsersController : Controller
    {
        AppDbContext db;

        public UsersController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {

            return View(db.Users.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            User user = db.Users.Find(id);

            if (user != null)
            {
                return PartialView("Edit", user);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            User user = db.Users.Find(id);

            if (user != null)
            {
                return PartialView("Delete", user);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Delete(User user)
        {
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("index");
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
