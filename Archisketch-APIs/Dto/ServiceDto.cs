using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos
{
    public class ServiceDto
    {
        public int Id { get; set; }

        [Required]
        public string Icon { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;
    }
}
