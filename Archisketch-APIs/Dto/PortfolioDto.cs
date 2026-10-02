using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos
{
    // Used for POST and PUT (multipart/form-data)
    public class PortfolioFormDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }

    // Used for GET responses
    public class PortfolioResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}