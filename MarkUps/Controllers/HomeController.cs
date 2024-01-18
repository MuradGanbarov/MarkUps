using MarkUps.DAL;
using MarkUps.Models;
using MarkUps.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MarkUps.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new()
            {
                Chefs = await _context.Chefs.Include(c => c.Position).ToListAsync(),
                Positions = await _context.Positions.ToListAsync(),
            };  
            return View(vm);
        }
    }
}
