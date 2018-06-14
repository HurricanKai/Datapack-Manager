using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client;
using Client.Models;

namespace Client.Controllers
{
    public class WorldsController : Controller
    {
        private readonly InfoDBContext _context;

        public WorldsController(InfoDBContext context)
        {
            _context = context;
        }

        // GET: Worlds
        public async Task<IActionResult> Index()
        {
            return View(await _context.Worlds.ToListAsync());
        }

        // GET: Worlds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldModel = await _context.Worlds
                .SingleOrDefaultAsync(m => m.Id == id);
            if (worldModel == null)
            {
                return NotFound();
            }

            return View(worldModel);
        }

        // GET: Worlds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Worlds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Path")] WorldModel worldModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(worldModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(worldModel);
        }

        // GET: Worlds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldModel = await _context.Worlds.SingleOrDefaultAsync(m => m.Id == id);
            if (worldModel == null)
            {
                return NotFound();
            }
            return View(worldModel);
        }

        // POST: Worlds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] WorldModel worldModel)
        {
            if (id != worldModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worldModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorldModelExists(worldModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(worldModel);
        }

        // GET: Worlds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worldModel = await _context.Worlds
                .SingleOrDefaultAsync(m => m.Id == id);
            if (worldModel == null)
            {
                return NotFound();
            }

            return View(worldModel);
        }

        // POST: Worlds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worldModel = await _context.Worlds.SingleOrDefaultAsync(m => m.Id == id);
            _context.Worlds.Remove(worldModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorldModelExists(int id)
        {
            return _context.Worlds.Any(e => e.Id == id);
        }
    }
}
