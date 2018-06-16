using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;

namespace Client
{
    public class Processing
    {
        public static void DoTagAndScoreboardThing(World w)
        {
            var FoundCommands = new Dictionary<Datapack, List<string>>();
            string TagPattern = @"tag @[a - z].+ add.+";
            string ScoreboardPattern = @"scoreboard objectives add.+";
            string path = w.Path + "/datapacks/data";
            try
            {
                foreach (var datapack in w.Datapacks)
                {
                    foreach (string s in Directory.EnumerateFiles(path + $"/{datapack.Name}", "*.mcfunction", SearchOption.AllDirectories))
                    {
                        var cmds = new List<string>();
                        var lines = new List<string>();
                        if (s != null)
                        {
                            using (var reader = new StreamReader(s))
                            {
                                while (!reader.EndOfStream)
                                {
                                    var text = reader.ReadLine();
                                    if (!text.StartsWith("#"))
                                        lines.Add(text);
                                }
                                reader.Dispose();
                            }
                            foreach (string line in lines)
                            {
                                foreach (Match match in Regex.Matches(line, TagPattern))
                                {
                                    if (!cmds.Any(x => x == match.Value)) cmds.Add(match.Value);
                                }
                                foreach (Match match in Regex.Matches(line, ScoreboardPattern))
                                {
                                    if (!cmds.Any(x => x == match.Value)) cmds.Add(match.Value);
                                }
                            }
                        }
                    }
                }
                foreach (var dp1 in FoundCommands)
                {
                    //TODO: Replace all same things
                    foreach (var dp2 in FoundCommands)
                    {
                        if (dp1.Key != dp2.Key)
                        {
                            var noneUnique = dp1.Value.Where(x => dp2.Value.Contains(x));
                            foreach (var n in noneUnique)
                            {
                                //DO stuff (all here parsing things are already the same. this is for shure.
                                string newName_1 = GetUniqueName(n, FoundCommands);
                                string newName_2 = GetUniqueName(n, FoundCommands, newName_1);
                                //now Replace all ocurrencys in dp1 of n with newName_1
                                //and the same with d2, n, and newName_2.

                                foreach (var file in Directory.EnumerateFiles(path + $"/{dp1.Key.Name}", "*.mcfunction", SearchOption.AllDirectories))
                                {
                                    var s = File.ReadAllText(file);
                                    File.WriteAllText(file, s.Replace(n, newName_1));
                                }

                                foreach (var file in Directory.EnumerateFiles(path + $"/{dp2.Key.Name}", "*.mcfunction", SearchOption.AllDirectories))
                                {
                                    var s = File.ReadAllText(file);
                                    File.WriteAllText(file, s.Replace(n, newName_2));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static String GetUniqueName(string old, Dictionary<Datapack, List<String>> foundCommands, params string[] ToIgnore)
        {
            var commands = foundCommands.SelectMany(x => x.Value).ToList();
            commands.AddRange(ToIgnore);
            int i = 1;
            while (true)
            {
                string newS = $"{old}_{i}";
                if (!commands.Contains(newS))
                    return newS;
                i++;
                if (i >= 100)
                    break;
            }
            return $"{old}_{Guid.NewGuid().ToString()}";
        }

        public static string GetColorCodeHTML(string s)
        {
            string s2 = s;
            string EndString = "";
            //Colors
            Dictionary<string, string> ColorMappings = new Dictionary<string, string>()
            {
                {"§0", "<span style=\"color: #000000\">" },
                {"§1", "<span style=\"color: #0000AA\">" },
                {"§2", "<span style=\"color: #00AA00\">" },
                {"§3", "<span style=\"color: #00AAAA\">" },
                {"§4", "<span style=\"color: #AA0000\">" },
                {"§5", "<span style=\"color: #AA00AA\">" },
                {"§6", "<span style=\"color: #FFAA00\">" },
                {"§7", "<span style=\"color: #AAAAAA\">" },
                {"§8", "<span style=\"color: #555555\">" },
                {"§9", "<span style=\"color: #5555FF\">" },
                {"§a", "<span style=\"color: #55FF55\">" },
                {"§b", "<span style=\"color: #55FFFF\">" },
                {"§c", "<span style=\"color: #FF5555\">" },
                {"§d", "<span style=\"color: #FF55FF\">" },
                {"§e", "<span style=\"color: #FFFF55\">" },
                {"§f", "<span style=\"color: #FFFFFF\">" },
                {"§k", "<span style=\"font-size: 0\">" },
                {"§l", "<span style=\"font-weight: bold\">" },
                {"§m", "<span style=\"text-decoration: line-through\">" },
                {"§o", "<span style=\"font-style: italic\">" },
            };

            foreach (var mapping in ColorMappings)
            {
                while (true)
                {
                    var index = s2.IndexOf(mapping.Key);
                    if (index < 0)
                        break;
                    s2 = s2.Remove(index, mapping.Key.Length);
                    s2 = s2.Insert(index, mapping.Value);
                    EndString += "</span>";
                }
            }

            while (true)
            {
                var index = s2.IndexOf("§r");
                if (index < 0)
                    break;
                s2 = s2.Remove(index, 2);
                s2 = s2.Insert(index, EndString);
                EndString = "";
            }

            s2 += EndString;


            return s2;
        }

        public static string WorldNameJSEncode(string s)
        {
            //var s2 = JavaScriptEncoder.Default.Encode(s);
            s = s.Replace(".", "u002E");
            s = s.Replace("[", "u005B");
            s = s.Replace("]", "u005D");
            s = s.Replace("'", "u0027");
            s = s.Replace("!", "u0021");
            s = s.Replace("?", "u003F");
            return s;
        }

        public static string UnEncode(string s)
        {
            s = s.Replace("u002E", ".");
            s = s.Replace("u005B", "[");
            s = s.Replace("u005D", "]");
            s = s.Replace("u0027", "'");
            s = s.Replace("u0021", "!");
            s = s.Replace("u003F", "?");
            return s;
        }
    }
}
