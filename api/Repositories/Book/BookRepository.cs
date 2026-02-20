using api.DTOs.Book;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Book
    {
    public class BookRepository : IBookRepository
        {
        private readonly ApplicationContext _context;

        public BookRepository(ApplicationContext context)
            {
            _context = context;
            }

        public async Task<BookDto> GetByIdAsync(int id)
            {
            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return null;

            var isAvailable = await CheckBookAvailabilityAsync(id);

            return MapToDto(book, isAvailable);
            }

        public async Task<PagedResultDto<BookDto>> GetAllAsync(BookFilterDto filter)
            {
            var query = _context.Books.Include(b => b.Genre).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Title))
                {
                query = query.Where(b => b.Title.ToLower().Contains(filter.Title.ToLower()));
                }

            if (!string.IsNullOrWhiteSpace(filter.Author))
                {
                query = query.Where(b => b.Author.ToLower().Contains(filter.Author.ToLower()));
                }

            if (filter.GenreId.HasValue)
                {
                query = query.Where(b => b.GenreId == filter.GenreId.Value);
                }

            var totalCount = await query.CountAsync();

            if (filter.PageSize <= 0)
                filter.PageSize = 10;
            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            var books = await query
                .OrderBy(b => b.Title)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var bookDtos = new List<BookDto>();
            foreach (var book in books)
                {
                var isAvailable = await CheckBookAvailabilityAsync(book.Id);
                bookDtos.Add(MapToDto(book, isAvailable));
                }

            return new PagedResultDto<BookDto>
                {
                Items = bookDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
                };
            }

        public async Task<BookDto> CreateAsync(CreateBookDto createBookDto)
            {
            var genreExists = await GenreExistsAsync(createBookDto.GenreId);
            if (!genreExists)
                return null;

            var book = new models.Book
                {
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                Description = createBookDto.Description,
                GenreId = createBookDto.GenreId,
                CoverImageUrl = createBookDto.CoverImageUrl,
                PublishedDate = createBookDto.PublishedDate
                };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(book.Id);
            }

        public async Task<BookDto> UpdateAsync(int id, UpdateBookDto updateBookDto)
            {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return null;

            var genreExists = await GenreExistsAsync(updateBookDto.GenreId);
            if (!genreExists)
                return null;

            book.Title = updateBookDto.Title;
            book.Author = updateBookDto.Author;
            book.Description = updateBookDto.Description;
            book.GenreId = updateBookDto.GenreId;
            book.CoverImageUrl = updateBookDto.CoverImageUrl;
            book.PublishedDate = updateBookDto.PublishedDate;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(book.Id);
            }

        public async Task<bool> DeleteAsync(int id)
            {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
            }

        public async Task<bool> BookExistsAsync(int id)
            {
            return await _context.Books.AnyAsync(b => b.Id == id);
            }

        public async Task<bool> GenreExistsAsync(int genreId)
            {
            return await _context.Genres.AnyAsync(g => g.Id == genreId);
            }

        private async Task<bool> CheckBookAvailabilityAsync(int bookId)
            {
            var hasActiveLoans = await _context.Loans
                .AnyAsync(l => l.BookId == bookId && l.ReturnDate == null);

            return !hasActiveLoans;
            }

        private static BookDto MapToDto(models.Book book, bool isAvailable = true)
            {
            return new BookDto
                {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name,
                CoverImageUrl = book.CoverImageUrl,
                PublishedDate = book.PublishedDate,
                IsAvailable = isAvailable
                };
            }
        }
    }
