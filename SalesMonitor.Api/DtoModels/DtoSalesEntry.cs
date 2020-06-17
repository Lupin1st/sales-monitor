using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SalesMonitor.Api.DtoModels
{
    public class DtoSalesEntry
    {
        [Required]
        [Description("The alphanumeric article number. Special characters are allowed.")]
        [StringLength(32)]
        public string ArticleNumber { get; set; }

        [Required]
        [Description("The price of the article in EUR.")]
        [Range(0, 1e6)]
        public double? Price { get; set; }
    }
}
