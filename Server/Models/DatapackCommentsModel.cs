using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class DatapackCommentsModel
    {
        [Key]
        public int Id { get; set; }
        public virtual UserModel Author { get; set; }
        public virtual DatapackModel Datapack { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; }
        public DateTime Creation { get; set; }
    }
}
