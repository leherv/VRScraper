using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using VRScraper.BO;

namespace VRScraper.Services
{
    [ApiController]
    [Route("api/release")]
    public class ScrapeService : IScrapeService
    {
        private readonly ILogger<ScrapeService> _logger;

        public ScrapeService(ILogger<ScrapeService> logger)
        {
            _logger = logger;
        }

        public async Task<Result<ScrapeResult>> Scrape(ScrapeInstruction scrapeInstruction)
        {
            var result = scrapeInstruction.MediaName.ToLower() switch
            {
                "sololeveling" => await ScrapeManganelo("https://manganelo.com/manga/pn918005", scrapeInstruction.MediaName.ToLower()),
                "talesofdemonsandgods" => await ScrapeManganelo("https://manganelo.com/manga/hyer5231574354229", scrapeInstruction.MediaName.ToLower()),
                "martialpeak" => await ScrapeManganelo("https://manganelo.com/manga/martial_peak", scrapeInstruction.MediaName.ToLower()),
                "jujutsukaisen" => await ScrapeManganelo("https://manganelo.com/manga/jujutsu_kaisen", scrapeInstruction.MediaName.ToLower()),
                _ => Result.Failure<ScrapeResult>($"No scraper set up for media with name {scrapeInstruction.MediaName}")
            };
            return result;
        }

        private async Task<Result<ScrapeResult>> ScrapeManganelo(string url, string mediaName)
        {
            try
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
                await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = true});
                var page = await browser.NewPageAsync();
                await page.GoToAsync(url);
                var container = await page.WaitForSelectorAsync("div.panel-story-chapter-list",
                    new WaitForSelectorOptions {Visible = true});
                var chapters = await container.QuerySelectorAsync("ul.row-content-chapter");
                var newestChapter = await chapters.QuerySelectorAsync("li.a-h");
                var newestLink = await newestChapter.QuerySelectorAsync("a");
                var chapterUrlHandle = await newestLink.GetPropertyAsync("href");
                var chapterUrl = (string) await chapterUrlHandle.JsonValueAsync();
                var regexResult = Regex.Match(chapterUrl, @"chapter_(\d{1,4})\.*(\d{0,4})");
                var releaseNumberString = regexResult.Groups[1].Value;
                var subReleaseNumberString = regexResult.Groups[2].Value;
                if (!int.TryParse(releaseNumberString, out var releaseNumber))
                {
                    _logger.LogError("Releasenumber could not be extracted from link {chapterUrl} for media {mediaName}", chapterUrl, mediaName);
                    return Result.Failure<ScrapeResult>($"Releasenumber could not be extracted from link {chapterUrl} for media {mediaName}");
                }

                var subReleaseNumber = 0;
                if (!string.IsNullOrEmpty(subReleaseNumberString))
                {
                    if (!int.TryParse(subReleaseNumberString, out subReleaseNumber))
                    {
                        return Result.Failure<ScrapeResult>($"SubReleaseNumber could not be extracted from link {chapterUrl} for media {mediaName}");
                    }
                }
                return Result.Success(new ScrapeResult
                {
                    MediaName = mediaName,
                    ReleaseNumber = releaseNumber,
                    SubReleaseNumber = subReleaseNumber,
                    Url = chapterUrl
                });
            }
            catch (Exception e)
            {
                _logger.LogError("Something went wrong while scraping for media {media}. {exceptionMessage}", mediaName, e.Message);
                return Result.Failure<ScrapeResult>($"Scraping for media {mediaName} failed due to {e.Message}");
            }
        }
    }
}