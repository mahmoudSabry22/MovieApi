using Microsoft.EntityFrameworkCore;
using MovieApi.Model;

namespace MovieApi.Services
{
    

    public class MoviesService : IMoviesService
    {
        private readonly ApplicationDbContext _context;

        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.AddAsync(movie);
            _context.SaveChanges();

            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
         return    await _context.Movies
                .Include(m => m.Genres)
                .Where(m => m.GenreId == genreId || genreId == 0)
                .OrderByDescending(g => g.Rate)
                .ToListAsync();
        }

        public async Task<Movie> GetById(int id)
        {
          return  await _context.Movies.Include(g => g.Genres).SingleOrDefaultAsync(m => m.Id == id);
        }

        public Movie Update(Movie movie)
        {
            _context.Update(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}
