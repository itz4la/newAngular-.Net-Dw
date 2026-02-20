using api.DTOs.Book;
using api.DTOs.Genre;
using api.Repositories.Genre;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
        {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
            {
            _genreRepository = genreRepository;
            }

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
            {
            var genres = await _genreRepository.GetAllAsync();
            return Ok(genres);
            }

        [HttpGet("paginated")]
        public async Task<ActionResult<PagedResultDto<GenreDto>>> GetAllWithPagination([FromQuery] GenreFilterDto filter)
            {
            var result = await _genreRepository.GetAllWithPaginationAsync(filter);
            return Ok(result);
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetById(int id)
            {
            if (id <= 0)
                return BadRequest("Invalid genre ID");

            var genre = await _genreRepository.GetByIdAsync(id);

            if (genre == null)
                return NotFound($"Genre with ID {id} not found");

            return Ok(genre);
            }

        [HttpPost]
        public async Task<ActionResult<GenreDto>> Create([FromBody] CreateGenreDto createGenreDto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreRepository.CreateAsync(createGenreDto);

            if (genre == null)
                return BadRequest("A genre with this name already exists");

            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
            }

        [HttpPut("{id}")]
        public async Task<ActionResult<GenreDto>> Update(int id, [FromBody] UpdateGenreDto updateGenreDto)
            {
            if (id <= 0)
                return BadRequest("Invalid genre ID");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreRepository.UpdateAsync(id, updateGenreDto);

            if (genre == null)
                {
                var genreExists = await _genreRepository.GenreExistsAsync(id);
                if (!genreExists)
                    return NotFound($"Genre with ID {id} not found");

                return BadRequest("A genre with this name already exists");
                }

            return Ok(genre);
            }
        }
    }
