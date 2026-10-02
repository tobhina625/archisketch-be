using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models
{
    public class Portfolio
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public byte[]? ImageData { get; set; }

        [MaxLength(100)]
        public string? ImageContentType { get; set; }
    }
}