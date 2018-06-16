using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using fNbt;
using fNbt.Serialization;
using fNbt.Tags;
using Client.Controllers;
using System.Text.RegularExpressions;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Client
{
    public class Startup
    {
        public static string ConfigPath = Path.GetFullPath("./Config.json");
        public static string DefaultMinecraftPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.minecraft";
        Task LoadingTask;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static async Task<Config> GetConfig()
        {
            var path = ConfigPath;
            if (File.Exists(path))
            {
                var text = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrEmpty(text))
                    return Config.FromJson(text);
            }
                File.Create(path).Close();
                var v = new Config();
                v.Save(path);
                return v;
        }

        private static async Task LoadWorlds(Config config)
        {
            if (config.Worlds.Count == 0)
            {
                ImportWorlds(DefaultMinecraftPath + "/saves/");
            }
            else
            {
                foreach (var c in config.Worlds)
                {
                    var c2 = c;
                    if (HomeController.Worlds.Any(x => x.Path == c2.Path)) continue;
                    c.DirectoryName = Path.GetFileName(c2.Path);
                    ReadWorldNbt(ref c2);
                    HomeController.Worlds.Add(c2);
                }
            }
        }

        public static void ImportWorlds(String path)
        {
            foreach (var world in Directory.EnumerateFiles(path, "level.dat", SearchOption.AllDirectories))
            {
                try
                {
                    var c = new World();
                    //world currently points to /saves/<name>/level.dat
                    //Then this points to /saves/<name>
                    var WorldDir = Path.GetDirectoryName(world);
                    if (HomeController.Worlds.Any(x => x.Path == WorldDir)) continue;
                    c.Path = WorldDir;
                    c.DirectoryName = Path.GetFileName(WorldDir);
                    ReadWorldNbt(ref c);
                    HomeController.Worlds.Add(c);
                }
                catch { }
            }
        }

        private static void ReadWorldNbt(ref World c)
        {
            var NbtPath = c.Path + "/level.dat";
            var File = new NbtFile();
            File.LoadFromFile(NbtPath);
            var compound = File.RootTag;
            if (compound.Tags.Count() > 1)
                c.LevelData = compound;
            else
                c.LevelData = (NbtCompound)compound.Tags.ToArray()[0];
            if (c.LevelData == null)
                throw new Exception();
            c.Name = c.LevelData.Get<NbtString>("LevelName").Value;
            try
            {
                c.Version = c.LevelData.Get<NbtCompound>("Version").Get("Name").StringValue;
            }
            catch
            {
                c.Version = "Unknown";
            }
        }

        private static async Task LoadDatapacks(Config config)
        {
            for (int i = 0; i < HomeController.Worlds.Count; i++)
            {
                var world = HomeController.Worlds[i];
                try
                {
                    var datapacks = world.LevelData.Get<NbtCompound>("DataPacks");
                    var disableds = datapacks.Get<NbtList>("Disabled");
                    var enableds = datapacks.Get<NbtList>("Enabled");
                    foreach (var disabled in disableds)
                    {
                        world.Datapacks.Add(new Datapack()
                        {
                            Enabled = false,
                            Name = disabled.StringValue
                        });
                    }
                    foreach (var enabled in enableds)
                    {
                        world.Datapacks.Add(new Datapack()
                        {
                            Enabled = true,
                            Name = enabled.StringValue
                        });
                    }
                }
                catch { }
                HomeController.Worlds[i] = world;
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            LoadingTask = Task.Run(async () =>
            {
                await Reload();
            });

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            Task.Run(async () =>
            {
                var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
                {
                    Show = false,

                    WebPreferences = new WebPreferences()
                    {
                        WebSecurity = false //TODO: This is very Dangerous!
                    }
                });
                window.OnReadyToShow += () => window.Show();
            });
            LoadingTask.Wait();
        }

        public static async Task Reload()
        {
            var config = await GetConfig();
            await LoadWorlds(config); //k done
            await LoadDatapacks(config);
            config.Save(ConfigPath);
        }

        public static void Save()
        {
            var config = new Config();
            config.Worlds = HomeController.Worlds;
            config.Save(ConfigPath);
        }
    }
}
