using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tm.Data.Models;

namespace Tm.Data.Interfaces
{
    public interface ISaleRepository
    {
        Task<IList<SalesSummary>> GetSaleSummaries(DateTime date);
        Task<IList<ProductsTotalAmount>> GetProductsTotalAmount(DateTime date);
    }
}
