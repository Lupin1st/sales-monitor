using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMonitor.Api.Data
{
    public class RepositoryCosmosDb : IRepository
    {
        private readonly SalesMonitorContext _context;

        public RepositoryCosmosDb(SalesMonitorContext context) {
            _context = context;
        }

        public async Task PreInitialize()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task AddSaleEntry(SalesEntry saleEntry)
        {
            _context.SalesEntries.Add(DboSalesEntry.FromModel(saleEntry));
            await _context.SaveChangesAsync();
        }

        public Task<int> GetNumberOfSoldArticles(DateTime from, DateTime to)
        {
            var entriesInRange = GetEntriesInRange(from: from, to: to);
            var articleCount = entriesInRange.Count();
            return Task.FromResult(articleCount);
        }

        public Task<double> GetTotalRevenue(DateTime from, DateTime to)
        {
            var entriesInRange = GetEntriesInRange(from: from, to: to);
            var revenue = entriesInRange.Sum(entry => entry.Price);
            return Task.FromResult(revenue);
        }

        public Task<SalesStatistics> GetStatistics(DateTime from, DateTime to)
        {
            var entriesInRange = GetEntriesInRange(from: from, to: to);
            var groupedEntries = entriesInRange.GroupBy(entry => entry.ArticleNumber);
            var statisticsArticleEntries = groupedEntries.Select(articleGroup =>
                new SalesStatisticsArticleEntry(articleNumber: articleGroup.Key,
                    revenue: articleGroup.Sum(article => article.Price))).ToArray();
            return Task.FromResult(new SalesStatistics(statisticsArticleEntries));
        }

        private IEnumerable<DboSalesEntry> GetEntriesInRange(DateTime from, DateTime to)
        {
            return _context.SalesEntries.AsNoTracking().Where(entry => entry.TimeOfSale >= from && entry.TimeOfSale < to);
        }
    }
}
