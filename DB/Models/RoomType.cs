using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public enum Type
    {
        Single, Double, Twin, Duplex, Suite
    }

    public partial class RoomType
    {
        public int Id { get; set; } 
        public Type Type { get; set; }
        public int Capacity { get; set; }
    }
}
