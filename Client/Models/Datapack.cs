using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Datapack : IEquatable<Datapack>
    {
        public Boolean Enabled { get; set; }
        public String Name { get; set; }

        public Boolean Equals(Datapack other)
        {
            return other.Name == this.Name;
        }
    }
}
