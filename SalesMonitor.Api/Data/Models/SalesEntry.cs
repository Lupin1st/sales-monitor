using SalesMonitor.Api.DtoModels;
using System;

namespace SalesMonitor.Api.Data
{
    public class SalesEntry
    {
        public SalesEntry(DateTime timeOfSale, string articleNumber, double price)
        {
            TimeOfSale = timeOfSale;
            ArticleNumber = articleNumber;
            Price = price;
        }

        public DateTime TimeOfSale { get; }

        public string ArticleNumber { get; }

        public double Price { get; }

        public static SalesEntry FromDto(DateTime timeOfSale, DtoSalesEntry dtoSalesEntry)
        {
            return new SalesEntry(timeOfSale: timeOfSale,
                articleNumber: dtoSalesEntry.ArticleNumber,
                price: dtoSalesEntry.Price.Value);
        }
    }
}
