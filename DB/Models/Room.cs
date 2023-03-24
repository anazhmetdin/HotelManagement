using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public partial class Room
    {
        public int Id { get; set; }
        [ForeignKey("RoomType")]
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
        public int Floor { get; set; }
        public int Number { get; set; }

        public override string ToString()
        {
            return $"{this.Number} ({this.RoomType.Type})";
        }
    }
}
