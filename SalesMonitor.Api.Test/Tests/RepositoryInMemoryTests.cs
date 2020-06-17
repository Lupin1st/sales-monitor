using SalesMonitor.Api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesMonitor.Api
{
    public class RepositoryInMemoryTests : RepositoryTestBase
    {
        private readonly List<SalesEntry> _salesEntries = new List<SalesEntry>();

        public override IRepository CreateRepository() => new RepositoryInMemory(_salesEntries);

        public override Task<IEnumerable<SalesEntry>> GetRepositoryContent()
        {
            return Task.FromResult(_salesEntries.AsEnumerable());
        }

        public override Task SetRepositoryContent(IEnumerable<SalesEntry> salesEntries)
        {
            _salesEntries.Clear();
            _salesEntries.AddRange(salesEntries);
            return Task.CompletedTask;
        }
    }
}
