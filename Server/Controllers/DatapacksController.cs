using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Models;

namespace Server.Controllers
{
    public class DatapacksController : Controller
    {
        private readonly DatapackDBContext _context;
        private readonly UserManager<UserModel> _userManager;

        public DatapacksController(DatapackDBContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> AddRelease(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Author).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            if (datapackModel.Author != await _userManager.GetUserAsync(User))
                return Forbid();
            ViewData["id"] = datapackModel.Id;
            return View();
        }

        public async Task<IActionResult> Download(int? id, string Version)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datapackModel = await _context.Datapacks.Where(m => m.Id == id).Include(x => x.Versions).SingleAsync();
            if (datapackModel == null)
            {
                return NotFound();
            }

            DatapackVersionModel version = null;

            if (!string.IsNullOrEmpty(Version))
                version = datapackModel.Versions.First(x => x.Name == Version);

            if (version == null)
            {
                version = datapackModel.Versions.OrderBy(x => x.ReleaseDate).ToList()[0];
            }

            var content = new FileStream(version.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var response = File(content, "application/octet-stream", $"{datapackModel.Name}-{version.Name}.zip");//FileStreamResult
            datapackModel.Downloads++;
            _context.Update(datapackModel);
            await _context.SaveChangesAsync();
            return response;
            //return RedirectToAction("Details/" + id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddRelease(int? id, [Bind("Name", "Notes", "PreRelease")] DatapackVersionModel Release, IFormFile file)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Author).Include(x => x.Versions).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            if (datapackModel.Author != await _userManager.GetUserAsync(User))
                return Forbid();

            if (!Directory.Exists("./Zips/"))
                Directory.CreateDirectory("./Zips");

            if (file != null)
            {
                string path = Path.GetFullPath($"./Zips/{datapackModel.Id}/{Release.Name}.zip");
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                Release.Path = path;
            }
            else
                return RedirectToAction("AddRelease/" + id);

            Release.ReleaseDate = DateTime.UtcNow;
            Release.Datapack = datapackModel;

            _context.Add(Release);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details/" + id);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteRelease(int? id, string version)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (version == null)
                return NotFound();

            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Author).Include(x => x.Versions).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            if (datapackModel.Author != await _userManager.GetUserAsync(User))
                return Forbid();


            var f = datapackModel.Versions.First(x => x.Name == version);
            if (f == null)
                return NotFound();

            datapackModel.Versions.Remove(f);
            _context.Update(datapackModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details/" + id);
        }

        // GET: Datapacks
        public async Task<IActionResult> Index(string search, int? pageIndex, string SortBy, bool? Descending)
        {
            var model = await _context.Datapacks.Include(x => x.Votes).Include(x => x.Author).Include(x => x.Tags).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                model = model.Where(x => x.Name.Contains(search) || x.Author.UserName == search || x.Tags.Any(x2 => x2.Tag == search)).ToList();
                ViewData["filter"] = search;
            }
            else
                ViewData["filter"] = "";

            if (SortBy == null)
                SortBy = "Votes";

            bool aDescending = Descending ?? false;

            switch (SortBy)
            {
                case "Votes":
                    if (aDescending)
                    {
                        model.OrderByDescending(x => x.LikeDiff * -1);
                    }
                    else
                        model.OrderBy(x => x.LikeDiff * -1);
                    break;
                case "Downloads":
                    if (aDescending)
                        model = model.OrderByDescending(x => x.Downloads).ToList();
                    else
                        model = model.OrderBy(x => x.Downloads).ToList();
                    break;
                case "Views":
                    if (aDescending)
                        model = model.OrderByDescending(x => x.Views).ToList();
                    else
                        model = model.OrderBy(x => x.Views).ToList();
                    break;
                case "AZ":
                    if (aDescending)
                        model = model.OrderByDescending(x => x.Name).ToList();
                    else
                        model = model.OrderBy(x => x.Name).ToList();
                    break;
                case "None":
                    break;
                default:
                    SortBy = "Votes";
                    goto case "Votes";
            }
            ViewData["SortBy"] = SortBy;
            ViewData["Descending"] = aDescending;

            return View("List", PaginatedList<DatapackModel>.Create(model, pageIndex ?? 1, 50));
        }

        // GET: Datapacks/Details/5 
        public async Task<IActionResult> Details(int? id, int? commentPage)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datapackModel = await _context.Datapacks.Where(x => x.Id == id).Include(x => x.Votes).Include(x => x.Author).Include(x => x.Versions).Include(x => x.Tags).Include(x => x.Comments).ThenInclude<DatapackModel, DatapackCommentsModel, UserModel>(x => x.Author)
                .SingleAsync();
            if (datapackModel == null)
            {
                return NotFound();
            }
            datapackModel.Versions = datapackModel.Versions.OrderByDescending(x => x.ReleaseDate).ToList();
            var currentUser = await _userManager.GetUserAsync(User);
            var comments = PaginatedList<DatapackCommentsModel>.Create(datapackModel.Comments, commentPage ?? 1, 10);
            datapackModel.Views++;
            var t = _context.Update(datapackModel);
            await _context.SaveChangesAsync();
            return View(new DetailsViewModel() { model = datapackModel, Comments = comments, IsOwner = (currentUser?.Id ?? "") == datapackModel.Author.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Comment(int id, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var datapackModel = await _context.Datapacks.Where(x => x.Id == id).Include(x => x.Comments).SingleAsync();
                datapackModel.Comments.Add(new DatapackCommentsModel()
                {
                    Author = await _userManager.GetUserAsync(User),
                    Creation = DateTime.UtcNow,
                    Datapack = datapackModel,
                    Message = message
                });
                _context.Update<DatapackModel>(datapackModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details/" + id);
        }

        // GET: Datapacks/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Datapacks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Description,Name")] DatapackModel datapackModel, string Tags)
        {
            datapackModel.Author = await _userManager.GetUserAsync(User);
            datapackModel.Category = "";
            datapackModel.Comments = new List<DatapackCommentsModel>();
            datapackModel.Downloads = 0;
            datapackModel.Votes = new List<DatapackVoteModel>();
            datapackModel.Versions = new List<DatapackVersionModel>();
            datapackModel.Views = 0;
            datapackModel.Tags = new List<DatapackTagModel>();
            if (!string.IsNullOrEmpty(Tags))
                foreach (var v in Tags.Split(","))
                {
                    datapackModel.Tags.Add(new DatapackTagModel() { Tag = v.Trim(), Datapack = datapackModel});
                }

            if (ModelState.IsValid)
            {
                _context.Add(datapackModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(datapackModel);
        }

        // GET: Datapacks/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Versions).Include(x => x.Author).Include(x => x.Tags).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            if (datapackModel.Author != await _userManager.GetUserAsync(User))
                return Forbid();
            return View(datapackModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Upvote(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var voter = await _userManager.GetUserAsync(User);
            if (voter == null)
                return Forbid();
            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Comments).Include(x => x.Votes).ThenInclude(x => x.User).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            var lastvote = datapackModel.Votes.FirstOrDefault(x => x.User == voter);
            if (lastvote != null)
            {
                datapackModel.Votes.Remove(lastvote);
                
            }

            datapackModel.Votes.Add(new DatapackVoteModel()
            {
                Datapack = datapackModel,
                User = voter,
                Value = 1
            });

            _context.Update(datapackModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details/" + id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Downvote(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var voter = await _userManager.GetUserAsync(User);
            if (voter == null)
                return Forbid();
            var datapackModel = _context.Datapacks.Where(m => m.Id == id).Include(x => x.Comments).Include(x => x.Votes).ThenInclude(x => x.User).Single();
            if (datapackModel == null)
            {
                return NotFound();
            }
            var lastvote = datapackModel.Votes.FirstOrDefault(x => x.User == voter);
            if (lastvote != null)
                datapackModel.Votes.Remove(lastvote);

            datapackModel.Votes.Add(new DatapackVoteModel()
            {
                Datapack = datapackModel,
                User = voter,
                Value = -1
            });

            _context.Update(datapackModel);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details/" + id);
        }

        // POST: Datapacks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Description,Name")] DatapackModel datapackModel, string Tags)
        {
            var datapackModel2 = _context.Datapacks.Where(x => x.Id == id).Include(x => x.Tags).Include(x => x.Author).Single();
            if (datapackModel2 == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (datapackModel2.Author != user)
                return Forbid();
            datapackModel2.Tags.RemoveAll(x => true);
            foreach (var v in Tags.Split(","))
            {
                datapackModel2.Tags.Add(new DatapackTagModel() { Tag = v.Trim(), Datapack = datapackModel2 });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datapackModel2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatapackModelExists(datapackModel2.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details/" + id);
            }
            else
                return NoContent();
        }

        // POST: Datapacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datapackModel = await _context.Datapacks.Where(x => x.Id == id).Include(x => x.Author).SingleOrDefaultAsync();
            var user = await _userManager.GetUserAsync(User);
            if (datapackModel.Author != user)
                return Forbid();
            _context.Datapacks.Remove(datapackModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatapackModelExists(int id)
        {
            return _context.Datapacks.Any(e => e.Id == id);
        }
    }
}
