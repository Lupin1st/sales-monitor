using Microsoft.AspNetCore.WebUtilities;
using Moq;
using SalesMonitor.Api.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SalesMonitor.Api
{
    public class ApiInformationTotalRevenueTests : ApiTestBase
    {
        private static string Uri = "information/revenue";

        public ApiInformationTotalRevenueTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("2000-01-01", HttpStatusCode.OK, 2.44)]
        [InlineData("a2000-01-01", HttpStatusCode.BadRequest, null)]
        [InlineData("2000-01-01T00:00:00Z", HttpStatusCode.BadRequest, null)]
        [InlineData(null, HttpStatusCode.OK, 4.4)]

        public async Task TestTotalRevenue(string day, HttpStatusCode expectedResponseStatusCode, double? expectedRevenue)
        {
            // Arrange
            var repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(repository => repository.GetTotalRevenue(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(expectedRevenue ?? 0.0)).Verifiable();

            var client = CreateApiClient(repositoryMock.Object);
            var request = new HttpRequestMessage(new HttpMethod("GET"), CreateRequestUrl(Uri, day));

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Content: {responseContent}");

            // Assert
            Assert.Equal(expectedResponseStatusCode, response.StatusCode);

            if (expectedResponseStatusCode == HttpStatusCode.OK)
            {
                var from = DateTime.MinValue;
                var to = DateTime.MaxValue;

                if (day != null)
                {
                    from = DateTime.Parse(day);
                    to = from.AddDays(1);
                }

                repositoryMock.Verify(mock => mock.GetTotalRevenue(
                    It.Is<DateTime>(arg => arg == from),
                    It.Is<DateTime>(arg => arg == to)),
                    Times.Once());
            }
        }

        private string CreateRequestUrl(string url, string day)
        {
            var queryParameters = new Dictionary<string, string>() { };
            if (day != null)
                queryParameters.Add("day", day);
            return QueryHelpers.AddQueryString(url, queryParameters);
        }
    }
}
