using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WebApplicationMVC.EntitiFramework;
using WebApplicationMVC.Models;

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

            return View(db.UsersMy.ToList());
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            db.UsersMy.Add(user);
            db.SaveChanges();
            return RedirectToAction("index");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            User user = db.UsersMy.Find(id);

            if (user != null)
            {
                return PartialView("Edit", user);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

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

        [Authorize]
        public IActionResult Delete(int id)
        {
            User user = db.UsersMy.Find(id);

            if (user != null)
            {
                return PartialView("Delete", user);
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(User user)
        {
            db.UsersMy.Remove(user);
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
