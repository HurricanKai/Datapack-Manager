using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ViewerModel
    {
        public int Id { get; set; }
        public DatapackModel Datapack { get; set; }
        public UserModel User { get; set; }
    }
}
