namespace SalesMonitor.Api.Data
{
    public class SalesStatisticsArticleEntry
    {
        public SalesStatisticsArticleEntry(string articleNumber, double revenue)
        {
            ArticleNumber = articleNumber;
            Revenue = revenue;
        }

        public string ArticleNumber { get; }

        public double Revenue { get; }
    }
}
