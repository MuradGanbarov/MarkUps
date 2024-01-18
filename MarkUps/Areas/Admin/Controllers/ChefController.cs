using MarkUps.Areas.Admin.Models.Utilities.Enums;
using MarkUps.Areas.Admin.Models.Utilities.Extentions;
using MarkUps.Areas.Admin.ViewModels;
using MarkUps.Areas.Admin.ViewModels.Pagination;
using MarkUps.DAL;
using MarkUps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarkUps.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChefController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ChefController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Chefs.CountAsync();
            List<Chef> chefs = await _context.Chefs.Include(c=>c.Position).Skip(page*3).Take(3).ToListAsync();

            PaginationVM<Chef> vm = new()
            {
                CurrentPage = page + 1,
                TotalPage = Math.Ceiling(count / 3),
                Items = chefs
            };
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            ChefCreateVM vm = new()
            {
                Positions = await _context.Positions.ToListAsync(),
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChefCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Positions = await _context.Positions.ToListAsync();
                return View(vm);
            }
            if (!vm.Photo.IsValidType(FileType.Image))
            {
                vm.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Image", "Photo should be image type");
                return View(vm);
            }
            if (!vm.Photo.IsValidSize(5,FileSize.Megabyte))
            {
                vm.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Image", "Photo can be less or equal 5mb");
                return View(vm);
            }

            Chef chef = new()
            {
                Name = vm.Name,
                Surname = vm.Surname,
                ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath,"assets","img","chef"),
                PositionId = vm.PositionId,
            };
            await _context.Chefs.AddAsync(chef);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Chef existed = await _context.Chefs.Include(c => c.Position).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();

            ChefUpdateVM vm = new()
            {
                Name = existed.Name,
                Surname = existed.Surname,
                ImageURL = existed.ImageURL,
                PositionId = existed.PositionId,
                Positions = await _context.Positions.ToListAsync()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,ChefUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Chef existed = await _context.Chefs.Include(c => c.Position).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            if(vm.Photo is not null)
            {
                if (!vm.Photo.IsValidType(FileType.Image))
                {
                    vm.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Image", "Photo should be image type");
                    return View(vm);
                }
                if (!vm.Photo.IsValidSize(5, FileSize.Megabyte))
                {
                    vm.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Image", "Photo can be less or equal 5mb");
                    return View(vm);
                }
                existed.ImageURL.Delete(_env.WebRootPath, "assets","img" ,"chef");
                existed.ImageURL = await vm.Photo.CreateAsync(_env.WebRootPath, "assets", "img", "chef");
            }
            existed.Name = vm.Name;
            existed.Surname = vm.Surname;
            existed.PositionId = vm.PositionId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return NotFound();
            Chef existed = await _context.Chefs.Include(c => c.Position).FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            existed.ImageURL.Delete(_env.WebRootPath, "assets", "img", "chef");
            _context.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
