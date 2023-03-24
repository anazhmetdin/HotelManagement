using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Models;

namespace DB.Manager
{
    public class ReservationManager: Manager<reservation>
    {
        public static void Init(IDbConnection _dbConnection)
        {
            dbConnection = _dbConnection;
            AddProc = "ReservationInsertCommand";
            DeleteProc = "ReservationDeleteCommand";
            GetAllQuery = "SELECT * FROM reservation";
            GetIdQuery = "SELECT * FROM reservation WHERE Id = @Id";
            UpdateProc = "ReservationUpdateCommand";
        }

        public static object? GetObject(reservation? t)
        {
            try
            {
            return new
            {
                t.towel,
                t.payment_type,
                t.leaving_time,
                t.guestSSN,
                t.Id,
                t.arrival_time,
                t.break_fast,
                t.cardId,
                t.check_in,
                t.roomId,
                t.supply_status,
                t.total_bill,
                t.cleaning,
                t.dinner,
                t.lunch,
                t.number_guest,
                t.s_surprise,
                t.food_bill
            };

            }
            catch { return null; }
        }

        public static bool Update(reservation? t, out long id)
        {
            return Update(GetObject(t), out id);
        }
    }
}
