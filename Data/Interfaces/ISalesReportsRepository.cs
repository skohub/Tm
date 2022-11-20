using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models.Sales;

namespace Data.Interfaces
{
    public interface ISalesReportsRepository
    {
        Task<IList<SalesSummary>> GetSaleSummaries(DateTime date);
        Task<IList<ProductsTotalAmount>> GetProductsTotalAmount(DateTime date);
        Task<Dictionary<string, decimal>> GetMonthlySales(int year, int month);
    }
}
