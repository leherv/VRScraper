using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using VRScraper.BO;

namespace VRScraper.Services
{
    public interface IScrapeService
    {
        Task<Result<ScrapeResult>> Scrape(ScrapeInstruction scrapeInstruction);
    }
}