using SalesMonitor.Api.Data;
using System;
using System.Net;

namespace SalesMonitor.Api
{
    public class StatisticsTestData
    {
        public StatisticsTestData(
            SalesStatistics repositoryContent,
            string from,
            string to,
            DateTime? expectedFrom,
            DateTime? expectedTo,
            HttpStatusCode responseStatusCode,
            string response)
        {
            RepositoryContent = repositoryContent;
            From = from;
            To = to;
            ExpectedFrom = expectedFrom;
            ExpectedTo = expectedTo;
            ResponseStatusCode = responseStatusCode;
            Response = response;
        }
        public SalesStatistics RepositoryContent { get; }
        public string From { get; }
        public string To { get; }
        public DateTime? ExpectedFrom { get; }
        public DateTime? ExpectedTo { get; }
        public HttpStatusCode ResponseStatusCode { get; }
        public string Response { get; }

        public override string ToString()
        {
            return $"{From} -> {To} : {ResponseStatusCode} # {Response}";
        }
    }
}
