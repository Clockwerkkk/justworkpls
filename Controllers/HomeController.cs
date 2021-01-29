using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await db.Persons.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            db.Persons.Add(person);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Sorti(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Person> persons = db.Persons.Include(x => x.Age);//возможно, все руинится вот на этом этапе из-за x => x.Age, 
            //потому что в изначальном коде было x => x.Company, где компания была отдельным классом

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["AgeSort"] = sortOrder == SortState.AgeAsc ? SortState.AgeDesc : SortState.AgeAsc;
            //ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;

            IOrderedQueryable<Person> orderedQueryables = sortOrder switch
            {
                SortState.NameAsc => persons.OrderBy(s => s.Name),
                SortState.NameDesc => persons.OrderByDescending(s => s.Name),
                SortState.AgeAsc => persons.OrderBy(s => s.Age),
                SortState.AgeDesc => persons.OrderByDescending(s => s.Age),
                //SortState.CompanyAsc => persons.OrderBy(s => s.Company.Name),
                //SortState.CompanyDesc => users.OrderByDescending(s => s.Company.Name),
                 _=> persons.OrderBy(s => s.Name),
            };
            persons = orderedQueryables;
            return View(await persons.AsNoTracking().ToListAsync());
        }
    }


}
