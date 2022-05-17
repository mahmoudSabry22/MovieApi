using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Model;
using Microsoft.EntityFrameworkCore;
using MovieApi.Dtos;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
       public async Task<IActionResult> GetAllAsync()
        {
            var TheGenres = await _context.Genres.OrderBy(g=>g.Name).ToListAsync();
            return Ok(TheGenres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            await _context.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] GenreDto dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);

            if (genre == null)
                return NotFound($" no genre found by id :{id}");

            genre.Name = dto.Name;
            _context.SaveChanges();

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var gener = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);

            if(gener == null)
                return NotFound($" no genre found by id :{id}");

            _context.Remove(gener);
            _context.SaveChanges();

            return Ok(gener);
        }
    }
}
