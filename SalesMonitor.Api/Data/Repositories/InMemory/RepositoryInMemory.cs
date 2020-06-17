using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMonitor.Api.Data
{
    public class RepositoryInMemory : IRepository
    {
        private List<SalesEntry> _entries;

        public RepositoryInMemory()
        {
            _entries = new List<SalesEntry>();
        }

        public RepositoryInMemory(List<SalesEntry> entries)
        {
            _entries = entries;
        }

        public Task PreInitialize()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public Task AddSaleEntry(SalesEntry saleEntry)
        {
            _entries.Add(saleEntry);
            return Task.CompletedTask;
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
                new SalesStatisticsArticleEntry( articleNumber: articleGroup.Key,
                    revenue: articleGroup.Sum(article => article.Price))).ToArray();
            return Task.FromResult(new SalesStatistics(statisticsArticleEntries));
        }

        private IEnumerable<SalesEntry> GetEntriesInRange(DateTime from, DateTime to)
        {
            return _entries.Where(entry => entry.TimeOfSale >= from && entry.TimeOfSale < to);
        }
    }
}
