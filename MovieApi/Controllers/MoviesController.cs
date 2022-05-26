using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Dtos;
using MovieApi.Model;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var TheMovies = await _context.Movies
                .Include(m => m.Genres)
                .Select(d => new MovieDetailsDto
                {
                    Id = d.Id,
                    GenreId = d.GenreId,
                    GenreName = d.Genres.Name,
                    Poster = d.Poster,
                    Rate = d.Rate,
                    Storeline = d.Storeline,
                    Title = d.Title,
                    Year = d.Year,

                })
                .OrderByDescending(g => g.Rate).ToListAsync();
            return Ok(TheMovies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.Include(g=>g.Genres).SingleOrDefaultAsync(m=>m.Id==id);

            if(movie == null)
                return NotFound();

            var dto = new MovieDetailsDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Rate = movie.Rate,
                Year = movie.Year,
                Storeline = movie.Storeline,
                Poster = movie.Poster,
                GenreId = movie.GenreId,
                GenreName = movie.Genres.Name
            };
            return  Ok(dto);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var TheMovies = await _context.Movies
                .Where(m=>m.GenreId == genreId)
                .Include(m => m.Genres)
                .Select(d => new MovieDetailsDto
                {
                    Id = d.Id,
                    GenreId = d.GenreId,
                    GenreName = d.Genres.Name,
                    Poster = d.Poster,
                    Rate = d.Rate,
                    Storeline = d.Storeline,
                    Title = d.Title,
                    Year = d.Year,

                })
                .OrderByDescending(g => g.Rate).ToListAsync();
            return Ok(TheMovies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required!");

            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var isValidGenre = await _context.Genres.AnyAsync(g=>g.Id == dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genere ID!");

            //i make that because type of dto.poster is iformfile and type op movie.poster is byte[]
            using var dateStrem = new MemoryStream();
            await dto.Poster.CopyToAsync(dateStrem);


            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Year = dto.Year,
                Storeline = dto.Storeline,
                Rate = dto.Rate,
                Poster = dateStrem.ToArray(),
            };

            await _context.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound($"No Movies was Found  By Id {id}");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid genere ID!");

            if(dto.Poster != null)
            {
                if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");

                using var dateStrem = new MemoryStream();
                await dto.Poster.CopyToAsync(dateStrem);
                movie.Poster = dateStrem.ToArray();
            }

            movie.Title = dto.Title;
            movie.Year = dto.Year;
            movie.Storeline = dto.Storeline;
            movie.Rate  = dto.Rate;
            movie.GenreId = dto.GenreId;

            _context.SaveChanges();

            return Ok(movie);
                

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound($"No Movies was Found  By Id {id}");

            _context.Remove(movie);
            _context.SaveChanges();

            return Ok(movie);
        }
    }
}
