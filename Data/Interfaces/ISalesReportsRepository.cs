using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tm.Data.Models;

namespace Tm.Data.Interfaces
{
    public interface ISalesReportsRepository
    {
        Task<IList<SalesSummary>> GetSaleSummaries(DateTime date);
        Task<IList<ProductsTotalAmount>> GetProductsTotalAmount(DateTime date);
        Task<Dictionary<string, decimal>> GetMonthlySales(int year, int month);
    }
}
