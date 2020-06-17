using System;
using System.Threading.Tasks;

namespace SalesMonitor.Api.Data
{
    public interface IRepository
    {
        Task PreInitialize();

        Task AddSaleEntry(SalesEntry saleEntry);

        Task<int> GetNumberOfSoldArticles(DateTime from, DateTime to);

        Task<double> GetTotalRevenue(DateTime from, DateTime to);

        Task<SalesStatistics> GetStatistics(DateTime from, DateTime to);
    }
}
