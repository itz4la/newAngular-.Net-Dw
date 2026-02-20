using api.DTOs.Book;
using api.DTOs.Genre;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Genre
    {
    public class GenreRepository : IGenreRepository
        {
        private readonly ApplicationContext _context;

        public GenreRepository(ApplicationContext context)
            {
            _context = context;
            }

        public async Task<List<GenreDto>> GetAllAsync()
            {
            var genres = await _context.Genres
                .Include(g => g.Books)
                .OrderBy(g => g.Name)
                .Select(g => MapToDto(g))
                .ToListAsync();

            return genres;
            }

        public async Task<PagedResultDto<GenreDto>> GetAllWithPaginationAsync(GenreFilterDto filter)
            {
            var query = _context.Genres.Include(g => g.Books).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                query = query.Where(g => g.Name.ToLower().Contains(filter.Name.ToLower()));
                }

            var totalCount = await query.CountAsync();

            if (filter.PageSize <= 0)
                filter.PageSize = 10;
            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            var genres = await query
                .OrderBy(g => g.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(g => MapToDto(g))
                .ToListAsync();

            return new PagedResultDto<GenreDto>
                {
                Items = genres,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
                };
            }

        public async Task<GenreDto> GetByIdAsync(int id)
            {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
                return null;

            return MapToDto(genre);
            }

        public async Task<GenreDto> CreateAsync(CreateGenreDto createGenreDto)
            {
            var nameExists = await GenreNameExistsAsync(createGenreDto.Name);
            if (nameExists)
                return null;

            var genre = new models.Genre
                {
                Name = createGenreDto.Name
                };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(genre.Id);
            }

        public async Task<GenreDto> UpdateAsync(int id, UpdateGenreDto updateGenreDto)
            {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return null;

            var nameExists = await GenreNameExistsAsync(updateGenreDto.Name, id);
            if (nameExists)
                return null;

            genre.Name = updateGenreDto.Name;

            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(genre.Id);
            }

        public async Task<bool> GenreExistsAsync(int id)
            {
            return await _context.Genres.AnyAsync(g => g.Id == id);
            }

        public async Task<bool> GenreNameExistsAsync(string name, int? excludeId = null)
            {
            var query = _context.Genres.Where(g => g.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                {
                query = query.Where(g => g.Id != excludeId.Value);
                }

            return await query.AnyAsync();
            }

        private static GenreDto MapToDto(models.Genre genre)
            {
            return new GenreDto
                {
                Id = genre.Id,
                Name = genre.Name,
                BooksCount = genre.Books?.Count ?? 0
                };
            }
        }
    }
