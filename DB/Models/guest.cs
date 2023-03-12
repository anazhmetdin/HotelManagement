using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Models;

public partial class guest
{
    public long SSN { get; set; }
    public string first_name { get; set; }

    public string last_name { get; set; }

    public DateTime birth_day { get; set; }

    public string gender { get; set; }

    public string phone_number { get; set; }

    public string email_address { get; set; }

    public string street_address { get; set; }

    public string apt_suite { get; set; }

    public string city { get; set; }

    public string state { get; set; }

    public string zip_code { get; set; }

    public virtual DbSet<card> cards { get; set; }
}
