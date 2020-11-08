using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VRScraper.BO;
using VRScraper.Config;
using VRScraper.DTO.request;
using VRScraper.DTO.response;
using VRScraper.Extensions;
using VRScraper.Services;

namespace VRScraper.Controllers
{
    [ApiController]
    [Route("api/scrape")]
    public class ScrapeController
    {
        private readonly ILogger<ScrapeController> _logger;
        private readonly TrackedMediaSettings _trackedMediaSettings;
        private readonly IScrapeService _scrapeService;

        public ScrapeController(ILogger<ScrapeController> logger, IOptions<TrackedMediaSettings> trackedMediaSettings, IScrapeService scrapeService)
        {
            _logger = logger;
            _scrapeService = scrapeService;
            _trackedMediaSettings = trackedMediaSettings.Value;
        }

        public async Task<JsonResult> Scrape([FromBody] ScrapeInstructionsDTO scrapeInstructionsDto)
        {
            try
            {
                var scrapeResults = new List<Result<ScrapeResult>>();
                // we do not use the automatic integration of fluentValidation into ASP.NET Core (validating objects that are passed in to controller actions), as we want to add ALL valid releases and not stop and throw if one is invalid)
                var scrapeInstructionValidator = new ScrapeInstructionValidator(_trackedMediaSettings);
                foreach (var scrapeInstruction in scrapeInstructionsDto.ScrapeInstructions)
                {
                    var validationResult = await scrapeInstructionValidator.ValidateAsync(scrapeInstruction);
                    if (validationResult.IsValid)
                    {
                        scrapeResults.Add(await _scrapeService.Scrape(new ScrapeInstruction(scrapeInstruction)));
                    }
                    else
                    {
                        scrapeResults.Add(Result.Failure<ScrapeResult>(validationResult.GetMessage()));
                    }
                }
                var results = scrapeResults
                    .Select(scrapeResult => scrapeResult.Map(s => new ScrapeResultDTO(s)))
                    .Select(scrapeResultDto => scrapeResultDto.AsSerializableResult());
                return new JsonResult(results);
            }
            catch (Exception e)
            {
                _logger.LogError("Something went wrong. {exceptionMessage}", e.Message);
                return new JsonResult(Result.Failure("Scraping for new releases failed.").AsSerializableResult());
            }
        }
    }
}