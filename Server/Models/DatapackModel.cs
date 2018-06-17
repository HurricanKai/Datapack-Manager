using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class DatapackModel
    {
        [Key]
        public Int32 Id { get; set; }
        public UserModel Author { get; set; }
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Category { get; set; }
        public virtual List<DatapackVersionModel> Versions { get; set; }
        public virtual List<DatapackCommentsModel> Comments { get; set; }
        public virtual List<DatapackTagModel> Tags { get; set; }
        public virtual List<DatapackVoteModel> Votes { get; set; }
        public Int32 Downloads { get; set; }
        public virtual List<ViewerModel> Viewers { get; set; }
        public Int32 Likes { get { return Votes?.Count(x => x.Value > 0) ?? 0; } }
        public Int32 Dislikes { get { return Votes?.Count(x => x.Value < 0) ?? 0; } }
        public Int32 LikeDiff { get { return Likes - Dislikes; } }
    }
}
