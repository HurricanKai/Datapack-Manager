using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class DatapackVersionModel
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Pre-Release")]
        public bool PreRelease { get; set; }
        public DateTime ReleaseDate { get; set; }
        public virtual DatapackModel Datapack { get; set; }
        [DisplayName("Name of Release")]
        public string Name { get; set; }
        [DisplayName("Release Notes")]
        [StringLength(2000)]
        public string Notes { get; set; }
        public string Path { get; set; }
    }
}
