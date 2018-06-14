using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class DatapackVoteModel
    {
        [Key]
        public int Id { get; set; }
        public UserModel User { get; set; }
        public DatapackModel Datapack { get; set; }
        public int Value { get; set; }
    }
}