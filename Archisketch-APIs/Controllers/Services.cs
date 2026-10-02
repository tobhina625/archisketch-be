using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Dtos;
using PortfolioApi.Models;

namespace PortfolioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Services : ControllerBase
    {
        private readonly AppDbContext _context;

        public Services(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAll()
        {
            return await _context.Services
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Icon = s.Icon,
                    Title = s.Title,
                    Text = s.Text
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetById(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null) return NotFound();

            return new ServiceDto
            {
                Id = service.Id,
                Icon = service.Icon,
                Title = service.Title,
                Text = service.Text
            };
        }

        [HttpPost]
        public async Task<ActionResult<ServiceDto>> Create([FromBody] ServiceDto dto)
        {
            var service = new Service
            {
                Icon = dto.Icon,
                Title = dto.Title,
                Text = dto.Text
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            dto.Id = service.Id;
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceDto dto)
        {
            var existing = await _context.Services.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Icon = dto.Icon;
            existing.Title = dto.Title;
            existing.Text = dto.Text;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
