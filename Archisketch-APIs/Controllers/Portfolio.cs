using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Portfolio : ControllerBase
    {
        private readonly AppDbContext _context;

        private const long MaxImageBytes = 2 * 1024 * 1024; // 2 MB
        private static readonly string[] AllowedTypes =
            { "image/jpeg", "image/png", "image/webp", "image/gif" };

        public Portfolio(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioResponseDto>>> GetAll()
        {
            // Project first so the image bytes are NOT loaded for the list
            var items = await _context.Portfolios
                .Select(p => new { p.Id, p.Title, p.Category, HasImage = p.ImageData != null })
                .ToListAsync();

            return items.Select(p => new PortfolioResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                ImageUrl = p.HasImage ? Url.Action(nameof(GetImage), new { id = p.Id }) : null
            }).ToList();
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<PortfolioResponseDto>> GetById(int id)
        {
            var p = await _context.Portfolios
                .Where(x => x.Id == id)
                .Select(x => new { x.Id, x.Title, x.Category, HasImage = x.ImageData != null })
                .FirstOrDefaultAsync();

            if (p == null) return NotFound();

            return new PortfolioResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Category = p.Category,
                ImageUrl = p.HasImage ? Url.Action(nameof(GetImage), new { id = p.Id }) : null
            };
        }

        [HttpGet("{id}/GetImage")]
        public async Task<IActionResult> GetImage(int id)
        {
            var p = await _context.Portfolios
                .Where(x => x.Id == id)
                .Select(x => new { x.ImageData, x.ImageContentType })
                .FirstOrDefaultAsync();

            if (p?.ImageData == null) return NotFound();

            return File(p.ImageData, p.ImageContentType ?? "application/octet-stream");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PortfolioResponseDto>> Create([FromForm] PortfolioFormDto dto)
        {
            var error = ValidateImage(dto.Image);
            if (error != null) return BadRequest(error);

            var portfolio = new Models.Portfolio
            {
                Title = dto.Title,
                Category = dto.Category
            };

            if (dto.Image != null)
            {
                await SetImageAsync(portfolio, dto.Image);
            }

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = portfolio.Id }, new PortfolioResponseDto
            {
                Id = portfolio.Id,
                Title = portfolio.Title,
                Category = portfolio.Category,
                ImageUrl = portfolio.ImageData != null
                    ? Url.Action(nameof(GetImage), new { id = portfolio.Id })
                    : null
            });
        }

        // PUT: api/portfolios/5
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] PortfolioFormDto dto)
        {
            var error = ValidateImage(dto.Image);
            if (error != null) return BadRequest(error);

            var existing = await _context.Portfolios.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Category = dto.Category;

            // Only replace the image if a new one was sent
            if (dto.Image != null)
            {
                await SetImageAsync(existing, dto.Image);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/portfolios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var portfolio = await _context.Portfolios.FindAsync(id);
            if (portfolio == null) return NotFound();

            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- helpers ----------

        private static string? ValidateImage(IFormFile? file)
        {
            if (file == null) return null; // image is optional

            if (file.Length == 0) return "Image file is empty.";
            if (file.Length > MaxImageBytes) return "Image must be 2 MB or smaller.";
            if (!AllowedTypes.Contains(file.ContentType.ToLowerInvariant()))
                return "Only JPEG, PNG, WebP, or GIF images are allowed.";

            return null;
        }

        private static async Task SetImageAsync(Models.Portfolio portfolio, IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            portfolio.ImageData = ms.ToArray();
            portfolio.ImageContentType = file.ContentType;
        }
    }
}