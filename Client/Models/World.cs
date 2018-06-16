using fNbt;
using fNbt.Tags;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Models
{
    public class World
    {
        public World()
        {
            Datapacks = new List<Datapack>();
        }

        [JsonProperty] public String Name { get; set; }
        [JsonProperty] public String Path { get; set; }
        [JsonIgnore] public NbtCompound LevelData { get; set; }
        [JsonIgnore] public List<Datapack> Datapacks { get; set; }
        [JsonIgnore] public string IconPath => Path + "/icon.png";
        [JsonIgnore] public String Version { get; set; }
        [JsonIgnore] public String DirectoryName { get; set; }
        [JsonIgnore] public string DirNameNoSpace => DirectoryName.Replace(' ', '_');

        public void SaveNbt()
        {
            var p = Path + "/level.dat";
            var file = new NbtFile(LevelData);
            if (File.Exists(p))
                File.Delete(p);
            file.SaveToFile(p, NbtCompression.None);
        }
    }
}