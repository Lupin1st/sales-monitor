using SalesMonitor.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SalesMonitor.Api
{
    public abstract class RepositoryTestBase
    {
        private readonly IRepository _repository;

        public abstract IRepository CreateRepository();

        public abstract Task<IEnumerable<SalesEntry>> GetRepositoryContent();

        public abstract Task SetRepositoryContent(IEnumerable<SalesEntry> salesEntries);

        public RepositoryTestBase()
        {
            _repository = CreateRepository();
        }

        private List<SalesEntry> CreateEntries() => new List<SalesEntry> {
                CreateSaleEntry("01/01/2000 00:00:00", 1, 1.1),
                CreateSaleEntry("01/01/2000 10:24:00", 1, 1.2),
                CreateSaleEntry("01/01/2000 11:44:00", 2, 2.1),
                CreateSaleEntry("01/01/2000 12:50:00", 3, 3.1),
                CreateSaleEntry("01/01/2000 23:49:00", 2, 2.1),
                CreateSaleEntry("02/01/2000 00:00:00", 2, 2.2),
                CreateSaleEntry("03/01/2000 00:10:00", 1, 1.3),
                CreateSaleEntry("04/01/2000 00:10:00", 4, 4.1),
            };

        [Fact]
        public async Task AddSalesEntryTest()
        {
            // Arrange
            var salesEntry1 = CreateSaleEntry("01/01/2000 00:10:00", 1, 1.1);
            var salesEntry2 = CreateSaleEntry("02/01/2000 00:10:00", 2, 2.2);

            // Act
            await _repository.AddSaleEntry(salesEntry1);
            await _repository.AddSaleEntry(salesEntry2);

            // Assert
            var repositoryContent = (await GetRepositoryContent()).OrderBy(e=>e.TimeOfSale);
            Assert.Equal(2, repositoryContent.Count());
            var dbSalesEntry2 = repositoryContent.Last();
            Assert.Equal("Article2", dbSalesEntry2.ArticleNumber);
            Assert.Equal(DateTime.Parse("02/01/2000 00:10:00"), dbSalesEntry2.TimeOfSale, 
                TimeSpan.FromSeconds(1));
            Assert.Equal(2.2, dbSalesEntry2.Price, 2);
        }

        [Fact]
        public async Task GetNumberOfSoldArticlesTest()
        {
            // Arrange
            await SetRepositoryContent(CreateEntries());

            // Act
            var numberOfSoldArticles = await _repository.GetNumberOfSoldArticles(
                from: DateTime.Parse("01/01/2000 00:00:00"), 
                to: DateTime.Parse("02/01/2000 00:00:00"));

            // Assert
            Assert.Equal(5, numberOfSoldArticles);
        }

        [Fact]
        public async Task GetTotalRevenueTest()
        {
            // Arrange
            await SetRepositoryContent(CreateEntries());

            // Act
            var totalRevenue = await _repository.GetTotalRevenue(
                from: DateTime.Parse("01/01/2000 00:00:00"), 
                to: DateTime.Parse("02/01/2000 00:00:00"));

            // Assert
            Assert.Equal(9.6, totalRevenue, 2);
        }

        [Fact]
        public async Task GetStatisticsTest()
        {
            // Arrange
            await SetRepositoryContent(CreateEntries());

            // Act
            var statistics = await _repository.GetStatistics(
                from: DateTime.Parse("01/01/2000 00:00:00"), 
                to: DateTime.Parse("03/01/2000 00:00:00"));

            // Assert
            Assert.Equal(3, statistics.Articles.Length);
            Assert.Equal(2.3, statistics.Articles.First(article => article.ArticleNumber == "Article1").Revenue, 2);
            Assert.Equal(6.4, statistics.Articles.First(article => article.ArticleNumber == "Article2").Revenue, 2);
            Assert.Equal(3.1, statistics.Articles.First(article => article.ArticleNumber == "Article3").Revenue, 2);
        }

        private SalesEntry CreateSaleEntry(string timeOfSale, int articleId, double price)
        {
            return new SalesEntry(timeOfSale: DateTime.Parse(timeOfSale), articleNumber: $"Article{articleId}", price: price);
        }
    }
}
