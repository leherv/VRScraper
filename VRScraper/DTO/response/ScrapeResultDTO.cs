using VRScraper.BO;

namespace VRScraper.DTO.response
{
    public class ScrapeResultDTO
    {
        public string MediaName { get; set; }
        public int ReleaseNumber { get; set; }
        public int? SubReleaseNumber { get; set; }
        public string Url { get; set; }

        public ScrapeResultDTO(ScrapeResult scrapeResult)
        {
            MediaName = scrapeResult.MediaName;
            ReleaseNumber = scrapeResult.ReleaseNumber;
            SubReleaseNumber = scrapeResult.SubReleaseNumber;
            Url = scrapeResult.Url;
        }
    }
}