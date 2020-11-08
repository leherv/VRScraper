using FluentValidation;
using VRScraper.Config;

namespace VRScraper.DTO.request
{
    public class ScrapeInstructionDTO
    {
        public string MediaName { get; set; }
    }
    
    public class ScrapeInstructionValidator : AbstractValidator<ScrapeInstructionDTO>
    {
        public ScrapeInstructionValidator(TrackedMediaSettings trackedMediaSettings)
        {
            RuleFor(sI => sI.MediaName)
                .Must(sI => trackedMediaSettings.MediaNames.Contains(sI.ToLower()))
                .WithMessage("Media of release to add is not included in the MediaSettings");
        }
    }
}