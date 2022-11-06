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
    public class SalesReportsRepository : ISalesReportsRepository
    {
        private readonly string _connectionString;

        public SalesReportsRepository(string connectionString)
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
                    "Date(sales.date) = Date(@date) " +
                    "and moveflag = 0 " +
                    "and places.organizationid = 1 " +
                "order by places.number, sales.date";
            
            using (var connection = new MySqlConnection(_connectionString))
            {
                var items = await connection.QueryAsync<SalesSummary>(sql, new { date });

                return items.ToList();
            }
        }

        public async Task<IList<ProductsTotalAmount>> GetProductsTotalAmount(DateTime date)
        {
            const string spName = "rest_sum_on_date";

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using (var connection = new MySqlConnection(_connectionString))
            {
                var amounts = await connection.QueryAsync<ProductsTotalAmount>(
                    spName, new { _date = date }, commandType: CommandType.StoredProcedure);

                return amounts.ToList();
            }
        }

        public async Task<Dictionary<string, decimal>> GetMonthlySales(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);
            const int organizationid = 1;
            const string sql = 
                "select " +
                    "get_user_name(saled_by) seller, " +
                    "sum(paid + debt) amount " +
                "from " +
                    "sales join places using(placeid) " +
                "where " +
                    "sales.moveflag = 0 and " +
                    "sales.date >= @startDate and " +
                    "sales.date < @endDate and " +
                    "places.organizationid = @organizationid " +
                "group by saled_by " +
                "order by amount desc";

            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = await connection.QueryAsync(sql, new { startDate, endDate, organizationid });

                return query.ToDictionary(x => (string) x.seller, x => (decimal) x.amount);
            }
        }
    }
}
