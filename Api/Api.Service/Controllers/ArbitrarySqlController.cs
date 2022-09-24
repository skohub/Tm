using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Tm.Data.Models;

namespace Api.Service.Controllers
{
    public class ArbitrarySqlController : Controller
    {
        private readonly IArbitrarySqlService _arbitrarySqlService;

        public ArbitrarySqlController(IArbitrarySqlService arbitrarySqlService)
        {
            _arbitrarySqlService = arbitrarySqlService;
        }

        public IActionResult Select(string sql, IList<SqlParameter> parameters)
        {
            var connectionString = "";
            var response = _arbitrarySqlService.Select(connectionString, sql);
            var result = new SqlResult();
            foreach (var responseRow in response)
            {
                var row = new SqlRow();
                foreach (var responseCol in responseRow)
                {
                    var c = (KeyValuePair<string, object>)responseCol;
                    row.Columns.Add(c.Key, c.Value);
                }
                result.Rows.Add(row);
            };

            return Json(result);
        }
    }
}
