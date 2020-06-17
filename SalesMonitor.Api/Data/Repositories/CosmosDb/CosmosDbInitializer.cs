using System.Threading.Tasks;

namespace SalesMonitor.Api.Data
{
    public static class CosmosDbInitializer
    {
        public static async Task Initialize(SalesMonitorContext context)
        {
            await context.Database.EnsureCreatedAsync();
        }
    }
}
