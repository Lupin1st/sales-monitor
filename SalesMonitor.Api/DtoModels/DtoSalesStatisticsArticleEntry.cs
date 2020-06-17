using SalesMonitor.Api.Data;
using System.ComponentModel;

namespace SalesMonitor.Api.DtoModels
{
    public class DtoSalesStatisticsArticleEntry
    {
        public DtoSalesStatisticsArticleEntry(SalesStatisticsArticleEntry entry)
        {
            ArticleNumber = entry.ArticleNumber;
            Revenue = entry.Revenue;
        }

        [Description("The article number.")]
        public string ArticleNumber { get; set; }

        [Description("The revenue of all sales for the article within the given date range.")]
        public double Revenue { get; set; }
    }
}
