namespace VRScraper.BO
{
    public class ScrapeResult
    {
        public string MediaName { get; set; } 
        public int ReleaseNumber { get; set; }
        public int? SubReleaseNumber { get; set; }
        public string Url { get; set; }
    }
}