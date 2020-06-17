using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodaTime;
using NSwag.Annotations;
using SalesMonitor.Api.Data;
using SalesMonitor.Api.DtoModels;

namespace SalesMonitor.Api.Controllers
{
    [ApiController]
    [Route("information")]
    [OpenApiTag("Information", Description = "Provides sales intelligence.")]
    public class InformationController : ControllerBase
    {
        private readonly ILogger<InformationController> _logger;
        private readonly IRepository _repository;

        public InformationController(ILogger<InformationController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("statistics")]
        [OpenApiOperation("Returns the revenue grouped by article. ",
            "Returns the revenue grouped by article. " +
            "The date range for the statistics can be limited by using the **from** and **to** query parameters.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoSalesStatisticsArticleEntry[]), Description = "Revenue statistics")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ValidationProblemDetails), Description = "Parameter validation error")]
        public async Task<ActionResult<DtoSalesStatisticsArticleEntry[]>> Statistics(
            [FromQuery]
            [Description("The minimum date and time to filter. " +
            "When not specified no minimum filter will be applied.")]
            OffsetDateTime? from,
            [FromQuery]
            [Description("The maximum date and time to filter. " +
            "When not specified no maximum filter will be applied.")]
            OffsetDateTime? to)
        {
            if (from == null)
                from = new OffsetDateTime(LocalDateTime.FromDateTime(DateTime.MinValue), Offset.Zero);

            if (to == null)
                to = new OffsetDateTime(LocalDateTime.FromDateTime(DateTime.MaxValue), Offset.Zero);

            try
            {
                var statistics = await _repository.GetStatistics(
                    from: from.Value.ToDateTimeOffset().UtcDateTime,
                    to: to.Value.ToDateTimeOffset().UtcDateTime);
                var result = statistics.Articles.Select(article =>
                    new DtoSalesStatisticsArticleEntry(article)).ToArray();
                _logger.LogInformation($"Successfully read sales statistics with {result.Length} articles.");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error reading the statistics from the repository.");
                throw;
            }
        }

        [HttpGet("articles-sold")]
        [OpenApiOperation("Returns the number of sold articles for a given day.",
            "Returns the number of sold articles for a given day. " +
            "If the **day** parameter is not specified the number of sold " +
            "articles for the whole sales history will be returned.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoSalesStatisticsArticleEntry[]), Description = "Articles sold")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ValidationProblemDetails), Description = "Parameter validation error")]
        public async Task<ActionResult<double>> ArticlesSold(
            [FromQuery]
            [Description("Filters the day the articles were bought. " +
            "When not specified no filter will be applied.")]
            LocalDate? day)
        {
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;

            if (day != null)
            {
                from = new DateTime(day.Value.Year, day.Value.Month, day.Value.Day);
                to = from.AddDays(1);
            }
            try
            {
                var numberOfSoldArticles = await _repository.GetNumberOfSoldArticles(from, to);
                _logger.LogInformation($"Successfully read the number of sold articles from the repository.");
                return Ok(numberOfSoldArticles);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while reading the number of sold articles from the repository.");
                throw;
            }
        }

        [HttpGet("revenue")]
        [OpenApiOperation("Returns the total revenue for a given day.",
            "Returns the total revenue for a given day." +
            "If the **day** parameter is not specified the total revenue " +
            "for the whole sales history will be returned.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DtoSalesStatisticsArticleEntry[]), Description = "Total revenue")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ValidationProblemDetails), Description = "Parameter validation error")]
        public async Task<ActionResult<int>> Revenue([FromQuery] LocalDate? day)
        {
            var from = DateTime.MinValue;
            var to = DateTime.MaxValue;

            if (day != null)
            {
                from = new DateTime(day.Value.Year, day.Value.Month, day.Value.Day);
                to = from.AddDays(1);
            }
            try
            {
                var numberOfSoldArticles = await _repository.GetTotalRevenue(from, to);
                _logger.LogInformation($"Successfully read the total revenue from the repository.");
                return Ok(numberOfSoldArticles);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while reading the total revenue from the repository.");
                throw;
            }
        }
    }
}
