using Dapper;
using DB.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Manager
{
    public class CardManager: Manager<card>
    {
        public static void Init(IDbConnection _dbConnection)
        {
            dbConnection = _dbConnection;
            AddProc = "CardInsertCommand";
            DeleteProc = "CardDeleteCommand";
            GetAllQuery = "SELECT * FROM Cards";
            GetIdQuery = "SELECT * FROM Cards WHERE Id = @Id";
            UpdateProc = "CardUpdateCommand";
        }
        public static object? GetObject(card? t)
        {
            try
            {
            return new
            {
                t.card_cvc,
                t.card_type,
                t.card_number,
                t.card_exp,
                t.guestSSN,
                t.Id
            };

            }
            catch { return null; }
        }

        public static bool Update(card? t, out long id)
        {
            return Update(GetObject(t), out id);
        }
    }
}
