using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SalesMonitor.Api.Data;
using System.Linq;
using System.Net.Http;
using Xunit.Abstractions;

namespace SalesMonitor.Api
{
    public abstract class ApiTestBase
    {
        protected readonly ITestOutputHelper _output;

        public ApiTestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        public HttpClient CreateApiClient(IRepository repositoryMock)
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
                    {
                        builder.UseEnvironment("Testing");
                        builder.ConfigureTestServices(services =>
                        {
                            var repositoryDescriptor = services.SingleOrDefault(
                                            d => d.ServiceType.IsAssignableFrom(typeof(IRepository)));

                            if (repositoryDescriptor != null)
                                services.Remove(repositoryDescriptor);

                            services.AddSingleton(serviceProvider => repositoryMock);
                        });
                    });

            return factory.CreateClient();
        }
    }
}
