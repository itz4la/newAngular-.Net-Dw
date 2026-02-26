using api.DTOs.Book;

namespace api.Repositories.Book
    {
    public interface IBookRepository
        {
        Task<BookDto> GetByIdAsync(int id);
        Task<PagedResultDto<BookDto>> GetAllAsync(BookFilterDto filter);
        Task<BookDto> CreateAsync(CreateBookDto createBookDto);
        Task<BookDto> UpdateAsync(int id, UpdateBookDto updateBookDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> BookExistsAsync(int id);
        Task<bool> GenreExistsAsync(int genreId);
        Task<List<BookDto>> GetAvailableBooksAsync();
        }
    }
