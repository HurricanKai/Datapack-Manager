using System;
using System.Collections.Generic; 
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server.Controllers
{
    [Route("api")] 
    [Produces("application/json")]
    public class APIController : Controller   //kann ich bei einem OpenFileDialog den Pfad zu einem String converten und dann in den StreamReader machen?
    {
        private DatapackDBContext _db;
        public APIController(DatapackDBContext db)
        {
            this._db = db;
        }

        [HttpGet("{id}")]
        [Route("Datapack")]
        public async Task<DatapackModel> GetById(int id)
        {
            return await this._db.FindAsync<DatapackModel>(id);
        }
    }
}

