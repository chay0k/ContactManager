using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApplication2.Resources;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private UserContext db;

        public HomeController(UserContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index(UserSortState sortOrder = UserSortState.NameAsc, string name = "", string id = "", string married = "", string phone = "")
        {
            IQueryable<User> users = db.Users;

            if (!String.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.Name.Contains(name));
            }
            if (!String.IsNullOrEmpty(id))
            {
                users = users.Where(p => p.Id.ToString().Contains(id));
            }
            if (!String.IsNullOrEmpty(married))
            {
                var isMarried = Convert.ToBoolean(married);
                users = users.Where(p => p.Married.Equals(isMarried));
            }
            if (!String.IsNullOrEmpty(phone))
            {
                users = users.Where(p => p.Phone.Contains(name));
            }

            ViewData["IdSort"] = sortOrder == UserSortState.IdAsc ? UserSortState.IdDesc : UserSortState.IdAsc;
            ViewData["NameSort"] = sortOrder == UserSortState.NameAsc ? UserSortState.NameDesc : UserSortState.NameAsc;
            ViewData["DateOfBirhSort"] = sortOrder == UserSortState.DateOfBirhAsc ? UserSortState.DateOfBirhDesc : UserSortState.DateOfBirhAsc;
            ViewData["MarriedSort"] = sortOrder == UserSortState.MarriedAsc ? UserSortState.MarriedDesc : UserSortState.MarriedAsc;
            ViewData["PhoneSort"] = sortOrder == UserSortState.PhoneAsc ? UserSortState.PhoneDesc : UserSortState.PhoneAsc;
            ViewData["SalarySort"] = sortOrder == UserSortState.SalaryAsc ? UserSortState.SalaryDesc : UserSortState.SalaryAsc;

            users = sortOrder switch
            {
                UserSortState.NameDesc => users.OrderByDescending(s => s.Name),
                UserSortState.IdAsc => users.OrderBy(s => s.Id),
                UserSortState.IdDesc => users.OrderByDescending(s => s.Id),
                UserSortState.DateOfBirhAsc => users.OrderBy(s => s.DateOfBirh),
                UserSortState.DateOfBirhDesc => users.OrderByDescending(s => s.DateOfBirh),
                UserSortState.MarriedAsc => users.OrderBy(s => s.Married),
                UserSortState.MarriedDesc => users.OrderByDescending(s => s.Married),
                UserSortState.PhoneAsc => users.OrderBy(s => s.Phone),
                UserSortState.PhoneDesc => users.OrderByDescending(s => s.Phone),
                UserSortState.SalaryAsc => users.OrderBy(s => s.Salary),
                UserSortState.SalaryDesc => users.OrderByDescending(s => s.Salary),
                _ => users.OrderBy(s => s.Name),
            };
            return View(await users.AsNoTracking().ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult LoadCsv()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadCsv(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // путь к папке Files
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                var loadData = new LoadUserData(db);
                loadData.ReadData(filePath);
                loadData.AddUserDataToDatabase();
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }
    }
}
