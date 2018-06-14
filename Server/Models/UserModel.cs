using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;  

namespace Server.Models
{
    public class UserModel : IdentityUser
    {
        public virtual List<DatapackModel> Datapacks { get; set; }
        public virtual List<DatapackCommentsModel> Comments { get; set; }
        public virtual List<DatapackVoteModel> Votes { get; set; }
    }
}
