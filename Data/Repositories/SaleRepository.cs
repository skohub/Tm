using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tm.Data.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using Tm.Data.Interfaces;
using System.Data;

namespace Tm.Data.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly string _connectionString;

        public SaleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IList<SalesSummary>> GetSaleSummaries(DateTime date)
        {
            const string sql =
                "select " +
                    "places.name as store, items.name as product, (sales.paid + sales.debt) as price " +
                "from " +
                    "sales " +
                    "join places using(placeid) " +
                    "join items on items.id = sales.itemid " +
                "where " +
                    "Date(sales.date) = Date(@Date) " +
                    "and moveflag = 0 " +
                    "and places.organizationid = 1 " +
                "order by places.number, sales.date";
            
            using (var connection = new MySqlConnection(_connectionString))
            {
                var items = await connection.QueryAsync<SalesSummary>(sql, new { Date = date });

                return items.ToList();
            }
        }

        public async Task<IList<ProductsTotalAmount>> GetProductsTotalAmount(DateTime date)
        {
            const string spName = "rest_sum_on_date";

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                var amounts = connection.Query<ProductsTotalAmount>(
                    spName, new { _date = date }, commandType: CommandType.StoredProcedure);

                return amounts.ToList();
            }
        }
    }
}
