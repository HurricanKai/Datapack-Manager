using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class DetailsViewModel
    {
        public DatapackModel model { get; set; }
        public bool IsOwner { get; set; }
        public PaginatedList<DatapackCommentsModel> Comments { get; set; }
    }
}
