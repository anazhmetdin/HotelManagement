using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models
{
    public partial class card
    {
        public int Id { get; set; }
        public string card_type { get; set; }

        public virtual guest guest { get; set; }

        public string card_number { get; set; }

        public string card_exp { get; set; }

        public string card_cvc { get; set; }

        public override string ToString()
        {
            return this.card_number;
        }
    }
}
