using MarkUps.Areas.Admin.ViewModels;
using MarkUps.Areas.Admin.ViewModels.Pagination;
using MarkUps.DAL;
using MarkUps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkUps.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Positions.CountAsync();
            List<Position> positions = await _context.Positions.Include(p => p.Chefs).Skip(page * 3).Take(3).ToListAsync();
            PaginationVM<Position> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = positions
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PositionCreateVM vm)
        {
            if (!ModelState.IsValid) return View();
            bool check = await _context.Positions.Include(p => p.Chefs).AnyAsync(p => p.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (check)
            {
                ModelState.AddModelError("Name", "This position already exists");
                return View();
            }
            Position position = new()
            {
                Name = vm.Name,
            };
            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Position existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            PositionUpdateVM vm = new()
            {
                Name = existed.Name,
            };
            return View(vm);
        }
        [HttpPost]

        public async Task<IActionResult> Update(int id, PositionUpdateVM vm)
        {
            if (!ModelState.IsValid) return View();
            Position existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            bool check = await _context.Positions.Include(p => p.Chefs).AnyAsync(p => p.Name.ToLower().Trim() == vm.Name.ToLower().Trim());
            if (check)
            {
                ModelState.AddModelError("Name", "This position already exists");
                return View();
            }
            existed.Name = vm.Name;
            await _context.SaveChangesAsync();
            return View(existed);

        }
        public async Task<IActionResult> Delete(int id)
        {

            if (!ModelState.IsValid) return View();
            Position existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();
            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
