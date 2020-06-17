using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using SalesMonitor.Api.Data;
using SalesMonitor.Api.DtoModels;

namespace SalesMonitor.Api.Controllers
{
    [ApiController]
    [Route("sales")]
    [OpenApiTag("Sales", Description = "Provides the ability to book sales")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly IRepository _repository;

        public SalesController(ILogger<SalesController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        [OpenApiOperation("Books the sale of an article.", "Books the sale of an article." +
            "The article will be booked for the current time of the day.")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "Successfully booked sales entry")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ValidationProblemDetails), Description = "Content validation error")]
        public async Task<ActionResult> Post(DtoSalesEntry entry)
        {
            try
            {
                var salesEntry = SalesEntry.FromDto(DateTime.UtcNow, entry);
                await _repository.AddSaleEntry(salesEntry);
                _logger.LogInformation($"Successfully added sales entry " +
                    $"{salesEntry.TimeOfSale}: {salesEntry.ArticleNumber}:{salesEntry.Price}.");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Adding sales entry failed.");
                throw;
            }
        }
    }
}
