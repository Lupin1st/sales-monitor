using System;

namespace SalesMonitor.Api.Data
{
    public class DboSalesEntry
    {
        public Guid Id { get; set; }

        public DateTime TimeOfSale { get; set; }

        public string ArticleNumber { get; set; }

        public double Price { get; set; }

        public static DboSalesEntry FromModel(SalesEntry salesEntry)
        {
            return new DboSalesEntry
            {
                Id = Guid.NewGuid(),
                TimeOfSale = salesEntry.TimeOfSale,
                ArticleNumber = salesEntry.ArticleNumber,
                Price = salesEntry.Price
            };
        }

        public SalesEntry ToModel()
        {
            return new SalesEntry(
                timeOfSale: TimeOfSale,
                articleNumber: ArticleNumber,
                price: Price);
        }
    }
}
