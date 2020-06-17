using Microsoft.EntityFrameworkCore;
using SalesMonitor.Api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMonitor.Api
{
    public class RepositoryCosmosDbTests : RepositoryTestBase
    {
        private static SalesMonitorContext Context = new SalesMonitorContext(new DbContextOptionsBuilder<SalesMonitorContext>()
                .UseInMemoryDatabase("sales_monitor_test_db")
                .Options);

        public override IRepository CreateRepository() => new RepositoryCosmosDb(Context);

        public override Task<IEnumerable<SalesEntry>> GetRepositoryContent()
        {
            return Task.FromResult(Context.SalesEntries.Select(e => e.ToModel()).
                ToArray().AsEnumerable());
        }

        public override async Task SetRepositoryContent(IEnumerable<SalesEntry> salesEntries)
        {
            Context.SalesEntries.RemoveRange(Context.SalesEntries);
            Context.SalesEntries.AddRange(salesEntries.Select(e => DboSalesEntry.FromModel(e)));
            await Context.SaveChangesAsync();
        }
    }
}
