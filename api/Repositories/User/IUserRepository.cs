using api.DTOs.Book;
using api.DTOs.User;

namespace api.Repositories.User
    {
    public interface IUserRepository
        {
        Task<PagedResultDto<UserDto>> GetAllAsync(UserFilterDto filter);
        Task<UserDto> GetByIdAsync(string id);
        Task<(bool Success, string Error, UserDto User)> CreateAdminAsync(CreateAdminUserDto dto);
        Task<(bool Success, string Error, UserDto User)> UpdateAsync(string id, UpdateUserDto dto);
        Task<(bool Success, string Error)> DeleteAsync(string id);
        }
    }
