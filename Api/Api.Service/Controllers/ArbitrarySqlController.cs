using Api.Service.Auth;
using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Service.Controllers
{
    [Route("api/arbitrarysql")]
    public class ArbitrarySqlController : Controller
    {
        private readonly IArbitrarySqlService _arbitrarySqlService;

        public ArbitrarySqlController(IArbitrarySqlService arbitrarySqlService)
        {
            _arbitrarySqlService = arbitrarySqlService;
        }

        [HttpGet("select")]
        public IActionResult Select(string sql, IList<SqlParameter> parameters)
        {
            if (!HttpContext.User.HasClaim(x => x.Type == TokenClaimTypes.ConnectionStringName))
            {
                return StatusCode(403);
            }

            var connectionStringName =
                HttpContext.User.FindFirst(x => x.Type == TokenClaimTypes.ConnectionStringName)!.Value;
            var response = _arbitrarySqlService.Select(connectionStringName, sql);
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
