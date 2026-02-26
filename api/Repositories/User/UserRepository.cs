using api.DTOs.Book;
using api.DTOs.User;
using api.models;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.User
    {
    public class UserRepository : IUserRepository
        {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
            _userManager = userManager;
            _roleManager = roleManager;
            }

        // ─── Helpers ────────────────────────────────────────────────────────

        private async Task EnsureRoleExistsAsync(string role)
            {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

        private static UserDto MapToDto(ApplicationUser user, string role) =>
            new UserDto
                {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
                };

        // ─── Queries ────────────────────────────────────────────────────────

        public async Task<PagedResultDto<UserDto>> GetAllAsync(UserFilterDto filter)
            {
            IList<ApplicationUser> users;

            if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                users = await _userManager.GetUsersInRoleAsync(filter.Role);
                }
            else
                {
                users = _userManager.Users.OrderBy(u => u.UserName).ToList();
                }

            if (!string.IsNullOrWhiteSpace(filter.UserName))
                {
                users = users
                    .Where(u => u.UserName != null &&
                                u.UserName.ToLower().Contains(filter.UserName.ToLower()))
                    .ToList();
                }

            var totalCount = users.Count;

            if (filter.PageSize <= 0) filter.PageSize = 10;
            if (filter.PageNumber <= 0) filter.PageNumber = 1;

            var paged = users
                .OrderBy(u => u.UserName)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtos = new List<UserDto>();
            foreach (var u in paged)
                {
                var roles = await _userManager.GetRolesAsync(u);
                dtos.Add(MapToDto(u, roles.FirstOrDefault() ?? ""));
                }

            return new PagedResultDto<UserDto>
                {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
                };
            }

        public async Task<UserDto> GetByIdAsync(string id)
            {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return MapToDto(user, roles.FirstOrDefault() ?? "");
            }

        // ─── Commands ───────────────────────────────────────────────────────

        public async Task<(bool Success, string Error, UserDto User)> CreateAdminAsync(CreateAdminUserDto dto)
            {
            await EnsureRoleExistsAsync("Admin");

            var existing = await _userManager.FindByNameAsync(dto.UserName);
            if (existing != null)
                return (false, "Username already exists", null);

            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

            await _userManager.AddToRoleAsync(user, "Admin");

            return (true, null, MapToDto(user, "Admin"));
            }

        public async Task<(bool Success, string Error, UserDto User)> UpdateAsync(string id, UpdateUserDto dto)
            {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "User not found", null);

            await EnsureRoleExistsAsync(dto.Role);

            // update username if changed
            if (user.UserName != dto.UserName)
                {
                var setName = await _userManager.SetUserNameAsync(user, dto.UserName);
                if (!setName.Succeeded)
                    return (false, string.Join(", ", setName.Errors.Select(e => e.Description)), null);
                }

            // update email if changed
            if (user.Email != dto.Email)
                {
                var setEmail = await _userManager.SetEmailAsync(user, dto.Email);
                if (!setEmail.Succeeded)
                    return (false, string.Join(", ", setEmail.Errors.Select(e => e.Description)), null);
                }

            // update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(dto.Role))
                {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, dto.Role);
                }

            return (true, null, MapToDto(user, dto.Role));
            }

        public async Task<(bool Success, string Error)> DeleteAsync(string id)
            {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return (false, "User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, null);
            }
        }
    }
