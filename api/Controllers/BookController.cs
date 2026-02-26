using api.DTOs.Book;
using api.Repositories.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
        {
        private readonly IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
            {
            _bookRepository = bookRepository;
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
            {
            if (id <= 0)
                return BadRequest("Invalid book ID");

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
                return NotFound($"Book with ID {id} not found");

            return Ok(book);
            }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<BookDto>>> GetAll([FromQuery] BookFilterDto filter)
            {
            var result = await _bookRepository.GetAllAsync(filter);
            return Ok(result);
            }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto createBookDto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookRepository.CreateAsync(createBookDto);

            if (book == null)
                return BadRequest("Genre not found. Please provide a valid Genre ID");

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
            }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] UpdateBookDto updateBookDto)
            {
            if (id <= 0)
                return BadRequest("Invalid book ID");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookRepository.UpdateAsync(id, updateBookDto);

            if (book == null)
                {
                var bookExists = await _bookRepository.BookExistsAsync(id);
                if (!bookExists)
                    return NotFound($"Book with ID {id} not found");

                return BadRequest("Genre not found. Please provide a valid Genre ID");
                }

            return Ok(book);
            }

        [HttpGet("available")]
        public async Task<ActionResult<List<BookDto>>> GetAvailable()
            {
            var books = await _bookRepository.GetAvailableBooksAsync();
            return Ok(books);
            }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
            {
            if (id <= 0)
                return BadRequest("Invalid book ID");

            var result = await _bookRepository.DeleteAsync(id);

            if (!result)
                return NotFound($"Book with ID {id} not found");

            return NoContent();
            }
        }
    }
