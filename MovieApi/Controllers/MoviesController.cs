﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Dtos;
using MovieApi.Model;
using MovieApi.Services;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenreService _genreService;

        private new List<string> _allowedExtenstions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(IMoviesService moviesService, IGenreService genreService)
        {

            _moviesService = moviesService;
            _genreService = genreService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var TheMovies =await _moviesService.GetAll();
            return Ok(TheMovies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

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
            var TheMovies = await _moviesService.GetAll(genreId);
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

            var isValidGenre = await _genreService.isValid(dto.GenreId);

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

           await  _moviesService.Add(movie);

            return Ok(movie);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound($"No Movies was Found  By Id {id}");

            var isValidGenre = await _genreService.isValid(dto.GenreId);
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

            _moviesService.Update(movie);

            return Ok(movie);
                

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound($"No Movies was Found  By Id {id}");
                
            _moviesService.Delete(movie);  
            return Ok(movie);
        }
    }
}
