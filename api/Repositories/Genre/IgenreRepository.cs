using api.DTOs.Book;
using api.DTOs.Genre;

namespace api.Repositories.Genre
    {
    public interface IGenreRepository
        {
        Task<List<GenreDto>> GetAllAsync();
        Task<PagedResultDto<GenreDto>> GetAllWithPaginationAsync(GenreFilterDto filter);
        Task<GenreDto> GetByIdAsync(int id);
        Task<GenreDto> CreateAsync(CreateGenreDto createGenreDto);
        Task<GenreDto> UpdateAsync(int id, UpdateGenreDto updateGenreDto);
        Task<bool> GenreExistsAsync(int id);
        Task<bool> GenreNameExistsAsync(string name, int? excludeId = null);
        }
    }
