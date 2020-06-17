using Moq;
using SalesMonitor.Api.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SalesMonitor.Api
{
    public class ApiSalesTests : ApiTestBase
    {
        private static string SalesUri = "/sales";

        public ApiSalesTests(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData("{'ArticleNumber':'Article1','Price':2.44}", HttpStatusCode.OK, "Article1", 2.44)]
        [InlineData("{'ArticleNumber':'','Price':2.44}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'ArticleNumber':'Article1'}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'ArticleNumber':'Article1#','Price':2.44}", HttpStatusCode.OK, "Article1#", 2.44)]
        [InlineData("{'ArticleNumber':'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx','Price':2.44}",
            HttpStatusCode.OK, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", 2.44)]
        [InlineData("{'ArticleNumber':'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx','Price':2.44}",
            HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'Price':2.44}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'Price':-0.01}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'Price':1000001}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'Price':'2.44'}", HttpStatusCode.BadRequest, null, null)]
        [InlineData("{'ArticleNumber':1,'Price':2.44}", HttpStatusCode.BadRequest, null, null)]

        public async Task TestSalesPosts(string content, HttpStatusCode expectedStatusCode,
            string expectedArticleNumber, double? expectedPrice)
        {
            // Arrange
            var repositoryMock = new Mock<IRepository>();
            repositoryMock.Setup(repository => repository.AddSaleEntry(It.IsAny<SalesEntry>())).Verifiable();

            var client = CreateApiClient(repositoryMock.Object);
            var request = new HttpRequestMessage(new HttpMethod("POST"), SalesUri)
            {
                Content = new StringContent(content: content.Replace('\'', '\"'),
                    encoding: Encoding.UTF8, mediaType: "application/json")
            };

            // Act
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Content: {responseContent}");

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);

            if (expectedStatusCode == HttpStatusCode.BadRequest)
            {
                repositoryMock.Verify(mock => mock.AddSaleEntry(It.IsAny<SalesEntry>()), Times.Never());
            }
            else
            {
                repositoryMock.Verify(mock => mock.AddSaleEntry(It.Is<SalesEntry>(arg =>
                arg.ArticleNumber == expectedArticleNumber &&
                Math.Abs(arg.Price - expectedPrice.Value) < 0.001)), Times.Once());
            }
        }
    }
}
