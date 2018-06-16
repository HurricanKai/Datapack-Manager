using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Net;
using fNbt.Tags;
using fNbt;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public static bool Initialized = false;
        public static void Init()
        {
            if (Initialized)
                return;

            Initialized = true;
        }

        public HomeController()
        {
            Init();
        }

        public static List<World> Worlds = new List<World>();
        public IActionResult Index()
        {
            return View(Worlds);
        }

        public async Task<IActionResult> MoveToTop(string id)
        {
            var world = Worlds.FirstOrDefault(x => x.Name == id);
            if (world == null)
                return RedirectToAction(nameof(Index));
            Worlds.Remove(world);

            Worlds.Insert(0, world);
            Startup.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddDatapack(string id)
        {
            try
            {
                var world = Worlds.FirstOrDefault(x => x.Name == id);
                if (world == null || !Directory.Exists(world.Path + "/datapacks"))
                    return RedirectToAction(nameof(Index));
                var index = Worlds.IndexOf(world);
                Worlds.Remove(world);

                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var options = new OpenDialogOptions
                {
                    Title = "Install Datapack",
                    Filters = new FileFilter[]
                    {
                    new FileFilter { Name = "Zip Files",
                                     Extensions = new string[] {"zip" } }
                    }
                };
                var res = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);

                var file = res[0];
                await AddDatapack(world, file);

                Worlds.Insert(index, world);
                Startup.Save();
            }
            catch { }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddDatapackDirect(string id, string Path)
        {
            id = WebUtility.UrlDecode(id);
            var world = Worlds.FirstOrDefault(x => x.Name == id);
            if (world == null || !Directory.Exists(world.Path + "/datapacks"))
            {
                ViewData["Message"] = "Error, Coudnt find world " + id;
                return View("Index", Worlds);
            }
            var index = Worlds.IndexOf(world);
            Worlds.Remove(world);
            await AddDatapack(world, Path);
            Worlds.Insert(index, world);
            Startup.Save();
            return RedirectToAction(nameof(Index));
        }

        public static async Task AddDatapack(World world, string file)
        {
            var tempPath = Path.GetTempPath() + "/" + Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + "DPMNGER"; //this shoud be unique every time!
            Directory.CreateDirectory(tempPath);
            ZipFile.ExtractToDirectory(file, tempPath);
            var root = GetDatapackRoot(tempPath);
            var name = Path.GetFileName(root);
            if (name == Path.GetFileName(tempPath))
                name = Path.GetFileNameWithoutExtension(file);
            var p = world.Path + $"/datapacks/{name}/";
            if (Directory.Exists(p))
            {
                var res = await Electron.Dialog.ShowMessageBoxAsync(new MessageBoxOptions("This Datapacks, or a Datapack with the Same name already is installed. Overwrite?")
                {
                    Buttons = new string[] { "Yes", "No" },
                    Title = "Overwrite?",
                });
                if (res.Response == 1)
                    return;
                else if (res.Response == 0)
                {
                    while (Directory.Exists(p))
                        try
                        {
                            Directory.Delete(p, true);
                        }
                        catch { }
                }
                else
                    throw new Exception();
            }
            Directory.Move(root, p);
            try
            {
                Directory.Delete(tempPath, true);
            }
            catch { }
            world.Datapacks.Add(new Datapack()
            {
                Enabled = true,
                Name = name
            });
            world.LevelData
                .Get<NbtCompound>("DataPacks")
                    .Get<NbtList>("Enabled")
                        .Add(new NbtString("file/" + name));
            world.SaveNbt();
        }

        public async Task<IActionResult> ImportWorlds()
        {
            try
            {
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var options = new OpenDialogOptions
                {
                    Title = "Install Datapack",
                    Filters = new FileFilter[]
                    {
                    new FileFilter { Name = "Zip Files",
                                     Extensions = new string[] {"zip" } }
                    },
                    Properties = new OpenDialogProperty[]
                    { OpenDialogProperty.openDirectory }
                };
                var res = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);

                var file = res[0];
                Startup.ImportWorlds(file);
                Startup.Save();
            }
            catch { }
            return RedirectToAction(nameof(Index));
        }

        private static string GetDatapackRoot(String tempPath)
        {
            return Path.GetDirectoryName(Directory.GetFiles(tempPath, "pack.mcmeta", SearchOption.AllDirectories)[0]);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static void CreateZipFromText(string text, string path)
        {
            byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(text);
            string encodedText = Convert.ToBase64String(byteArray);
            FileStream destFile = System.IO.File.Create(path);

            byte[] buffer = Encoding.UTF8.GetBytes(encodedText);
            MemoryStream memoryStream = new MemoryStream();

            using (System.IO.Compression.GZipStream gZipStream = new System.IO.Compression.GZipStream(destFile, System.IO.Compression.CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
