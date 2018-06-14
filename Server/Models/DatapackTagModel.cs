using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class DatapackTagModel
    {
        [Key]
        public int Id { get; set; }
        public DatapackModel Datapack { get; set; }
        public string Tag { get; set; }
    }
}
