namespace SalesMonitor.Api.Data
{
    public class SalesStatistics
    {
        public SalesStatistics(SalesStatisticsArticleEntry[] articles)
        {
            Articles = articles;
        }

        public SalesStatisticsArticleEntry[] Articles { get; }
    }
}
