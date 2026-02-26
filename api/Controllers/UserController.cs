using api.models;
using api.DTOs.User;
using api.Repositories.User;
using domaine;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
        {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserController(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
            {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            }

        // ─── Public Auth ──────────────────────────────────────────────────

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
            {
            var existing = await _userManager.FindByNameAsync(registerDTO.Username);
            if (existing != null)
                return Conflict(new { message = "Username already exists." });

            var user = new ApplicationUser { UserName = registerDTO.Username, Email = registerDTO.Email };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
                {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = errors });
                }

            if (!await _roleManager.RoleExistsAsync("Client"))
                await _roleManager.CreateAsync(new IdentityRole("Client"));

            await _userManager.AddToRoleAsync(user, "Client");
            return Ok(new { message = "Account created successfully." });
            }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
            {
            var user = await _userManager.FindByNameAsync(loginDTO.Username);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
                return BadRequest("Invalid username or password");

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
                {
                new Claim("username", user.UserName),
                new Claim("userId", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            foreach (var role in roles)
                claims.Add(new Claim("role", role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: sc);

            return Ok(new
                {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = loginDTO.Username
                });
            }

        // ─── Admin CRUD ───────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<api.DTOs.Book.PagedResultDto<UserDto>>> GetAll([FromQuery] UserFilterDto filter)
            {
            var result = await _userRepository.GetAllAsync(filter);
            return Ok(result);
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
            {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found");
            return Ok(user);
            }

        [HttpGet("non-admin")]
        public async Task<ActionResult<List<UserDto>>> GetNonAdminUsers()
            {
            var filter = new UserFilterDto { PageNumber = 1, PageSize = 1000 };
            var all = await _userRepository.GetAllAsync(filter);
            var nonAdmins = all.Items.Where(u => u.Role != "Admin").ToList();
            return Ok(nonAdmins);
            }

        [HttpPost("admin")]
        public async Task<ActionResult<UserDto>> CreateAdmin([FromBody] CreateAdminUserDto dto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, user) = await _userRepository.CreateAdminAsync(dto);
            if (!success)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(string id, [FromBody] UpdateUserDto dto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error, user) = await _userRepository.UpdateAsync(id, dto);
            if (!success)
                return error == "User not found" ? NotFound(error) : BadRequest(error);

            return Ok(user);
            }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
            {
            var (success, error) = await _userRepository.DeleteAsync(id);
            if (!success)
                return error == "User not found" ? NotFound(error) : BadRequest(error);

            return NoContent();
            }
        }
    }
