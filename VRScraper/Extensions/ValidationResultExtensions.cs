using System.Linq;
using FluentValidation.Results;

namespace VRScraper.Extensions
{
    public static class ValidationResultExtensions
    {
        public static string GetMessage(this ValidationResult validationResult)
        {
            return string.Join(";", validationResult.Errors.Select(error => error.ErrorMessage));
        }
    }
}