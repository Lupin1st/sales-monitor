using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using Newtonsoft.Json.Linq;
using SalesMonitor.Api.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace SalesMonitor.Api
{
    public class ApiInformationStatisticsTests : ApiTestBase
    {
        private static string Uri = "information/statistics";

        public ApiInformationStatisticsTests(ITestOutputHelper output) : base(output) { }

        public static IEnumerable<object[]> GetStatisticsTestData()
        {
            yield return new object[] { new StatisticsTestData(
                repositoryContent: new SalesStatistics(
                    articles: new SalesStatisticsArticleEntry[]{
                    new SalesStatisticsArticleEntry("Article1", 123.5),
                    new SalesStatisticsArticleEntry("Article2", 42.7)
                    }
                ),
                from: "2000-01-01T00:00:00Z",
                to: "2000-01-03T00:00:00Z",
                expectedFrom: new DateTime(2000,1,1,0,0,0),
                expectedTo: new DateTime(2000,1,3,0,0,0),
                responseStatusCode: HttpStatusCode.OK,
                response: "[{'articleName':'Article1', 'price':123.5}," +
                    "{'articleName':'Article2', 'price':42.7}]"
            )};

            yield return new object[] { new StatisticsTestData(
                repositoryContent: new SalesStatistics(
                    articles: new SalesStatisticsArticleEntry[]{
                    new SalesStatisticsArticleEntry("Article1", 123.5),
                    new SalesStatisticsArticleEntry("Article2", 42.7)
                    }
                ),
                from: null,
                to: "2000-01-03T00:00:00Z",
                expectedFrom: DateTime.MinValue,
                expectedTo: new DateTime(2000,1,3,0,0,0),
                responseStatusCode: HttpStatusCode.OK,
                response: "[{'articleName':'Article1', 'price':123.5}," +
                    "{'articleName':'Article2', 'price':42.7}]"
            )};

            yield return new object[] { new StatisticsTestData(
                repositoryContent: new SalesStatistics(
                    articles: new SalesStatisticsArticleEntry[]{
                    new SalesStatisticsArticleEntry("Article1", 123.5),
                    new SalesStatisticsArticleEntry("Article2", 42.7)
                    }
                ),
                from: "2000-01-01T00:00:00Z",
                to: null,
                expectedFrom: new DateTime(2000,1,1,0,0,0),
                expectedTo: DateTime.MaxValue,
                responseStatusCode: HttpStatusCode.OK,
                response: "[{'articleName':'Article1', 'price':123.5}," +
                    "{'articleName':'Article2', 'price':42.7}]"
            )};

            yield return new object[] { new StatisticsTestData(
                 new SalesStatistics(
                    articles: new SalesStatisticsArticleEntry[]{}
                ),
                from: "a2000-01-01T00:00:00Z",
                to: "2000-01-03T00:00:00Z",
                expectedFrom: null,
                expectedTo: null,
                responseStatusCode: HttpStatusCode.BadRequest,
                response: "[]"
            )};

            yield return new object[] { new StatisticsTestData(
                 repositoryContent: new SalesStatistics(
                    articles: new SalesStatisticsArticleEntry[]{
                    new SalesStatisticsArticleEntry("Article1", 123.5),
                    new SalesStatisticsArticleEntry("Article2", 42.7)
                    }
                ),
                from: "2000-01-01T00:00:00-04:00",
                to: "2000-01-03T00:00:00+02:00",
                expectedFrom: new DateTime(2000,1,1,4,0,0),
                expectedTo: new DateTime(2000,1,2,22,0,0),
                responseStatusCode: HttpStatusCode.OK,
                response: "[{'articleName':'Article1', 'price':123.5}," +
                    "{'articleName':'Article2', 'price':42.7}]"
            )};
        }

        [Theory]
        [MemberData(nameof(GetStatisticsTestData))]
        public async Task TestStatistics(StatisticsTestData data)
        {
            // Arrange
            var repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(repository => repository.GetStatistics(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(data.RepositoryContent)).Verifiable();

            var client = CreateApiClient(repositoryMock.Object);
            var request = new HttpRequestMessage(new HttpMethod("GET"),
                CreateRequestUrl(Uri, data.From, data.To));

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Content: {responseContent}");

            // Assert
            Assert.Equal(data.ResponseStatusCode, response.StatusCode);

            if (data.ResponseStatusCode == HttpStatusCode.OK)
            {
                var actual = JToken.Parse(responseContent);
                var expected = JToken.Parse(data.Response.Replace('\'', '\"'));
                actual.Should().BeEquivalentTo(expected);

                DateTime.SpecifyKind(data.ExpectedFrom.Value, DateTimeKind.Utc);
                DateTime.SpecifyKind(data.ExpectedTo.Value, DateTimeKind.Utc);

                repositoryMock.Verify(mock => mock.GetStatistics(
                    It.Is<DateTime>(arg => arg == data.ExpectedFrom.Value),
                    It.Is<DateTime>(arg => arg == data.ExpectedTo.Value)),
                    Times.Once());
            }
        }

        private string CreateRequestUrl(string url, string from, string to)
        {
            var queryParameters = new Dictionary<string, string>() { };
            if (from != null)
                queryParameters.Add("from", from);
            if (to != null)
                queryParameters.Add("to", to);

            return QueryHelpers.AddQueryString(url, queryParameters);
        }
    }
}
