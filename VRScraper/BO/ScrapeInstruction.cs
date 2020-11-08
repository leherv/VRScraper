using VRScraper.DTO;
using VRScraper.DTO.request;

namespace VRScraper.BO
{
    public class ScrapeInstruction
    {
        public string MediaName { get; set; }

        public ScrapeInstruction(ScrapeInstructionDTO scrapeInstructionDto)
        {
            MediaName = scrapeInstructionDto.MediaName.ToLower();
        }
    }
}