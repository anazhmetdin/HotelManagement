using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using System.Numerics;

namespace DB.Manager
{
    public abstract class Manager<T>
    {
        public static IDbConnection? dbConnection;
        public static string AddProc { get; set; } = string.Empty;
        public static string DeleteProc { get; set; } = string.Empty;
        public static string UpdateProc { get; set; } = string.Empty;
        public static string GetAllQuery { get; set; } = string.Empty;
        public static string GetIdQuery { get; set; } = string.Empty;

        public static bool Add(T Item, out Int64 id)
        {
            return AddUpdate(Item, out id, AddProc);

        }

        public static bool Delete(long Id)
            => dbConnection.Execute(DeleteProc, new { @Id = Id },
                commandType: CommandType.StoredProcedure) > 0;

        public static List<T> GetALL() =>
            dbConnection.Query<T>(GetAllQuery)?.ToList() ?? new();

        public static T GetByID(long Id) =>
            dbConnection.QueryFirstOrDefault<T>(GetIdQuery,
                new { Id });

        public static bool Update(object Item, out Int64 id)
        {
            return AddUpdate(Item, out id, UpdateProc);
        }

        static private bool AddUpdate(object Item, out Int64 id, string command)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("oid", dbType: DbType.Int64, direction: ParameterDirection.Output);
                var properties = Item?.GetType().GetProperties();
                foreach (var prop in properties ?? default!)
                {
                    var key = prop.Name;
                    var value = prop.GetValue(Item);

                    p.Add(key, value);
                }
                var res = dbConnection.Execute(command, p, commandType: CommandType.StoredProcedure) > 0;
                id = p.Get<Int64>("oid");
                return res;
            }
            catch
            {
                id = -1;
                return false;
            }
        }
    }
}
