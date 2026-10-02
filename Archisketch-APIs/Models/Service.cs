using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Icon { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Text { get; set; } = string.Empty;
    }
}
