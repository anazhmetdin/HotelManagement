using DB.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Manager
{
    public class GuestManager : Manager<guest>
    {
        public static void Init(IDbConnection _dbConnection)
        {
            dbConnection = _dbConnection;
            AddProc = "GuestInsertCommand";
            DeleteProc = "GuestDeleteCommand";
            GetAllQuery = "SELECT * FROM guest";
            GetIdQuery = "SELECT * FROM guest WHERE SSN = @Id";
            UpdateProc = "GuestUpdateCommand";
        }

        public static object? GetObject(guest? t)
        {
            try
            {
            return new
            {
                t.gender,
                t.street_address,
                t.city,
                t.state,
                t.SSN,
                t.apt_suite,
                t.first_name,
                t.last_name,
                t.phone_number,
                t.email_address,
                t.zip_code,
                t.birth_day
            };

            }
            catch { return null; }
        }

        public static bool Update(guest? t, out long id)
        {
            return Update(GetObject(t), out id);
        }
    }
}
