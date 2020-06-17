using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMonitor.Api.Data
{
    public static class RepositoryInitializer
    {
        private const int NumberOfSampleEntryDays = 30;

        public static async Task InitializeRepository(IRepository repository, ILogger logger)
        {
            await repository.PreInitialize();

            if (await repository.GetNumberOfSoldArticles(DateTime.MinValue, DateTime.MaxValue) == 0)
            {
                logger.LogInformation("About to seed the repository...");

                foreach (var entry in CreateSampleData(repository))
                    await repository.AddSaleEntry(entry);
            }
        }

        private static IEnumerable<SalesEntry> CreateSampleData(IRepository repository)
        {
            var randon = new Random(42);
            var now = DateTime.UtcNow;
            return Enumerable.Range(0, NumberOfSampleEntryDays * 24).Select(hourOffset =>
            {
                var articleId = 1 + hourOffset % 5;
                var price = Math.Round(randon.NextDouble() * 100, 2);
                return new SalesEntry(now.AddHours(-hourOffset), $"Article{articleId}", price);
            });
        }
    }
}
