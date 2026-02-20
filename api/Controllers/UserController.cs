using api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using domaine;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
        {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
            {
            this.userManager = userManager;
            this.configuration = configuration;
            }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
            {
            var user = await userManager.FindByNameAsync(registerDTO.Username);
            if (user != null)
                {
                return BadRequest("Username already exists");
                }
            var newUser = new ApplicationUser
                {
                UserName = registerDTO.Username,
                Email = registerDTO.Email
                };
            var result = await userManager.CreateAsync(newUser, registerDTO.Password);
            if (result.Succeeded)
                {
                return Ok("User registered successfully");
                }
            else
                {
                return BadRequest("User registration failed");
                }
            }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
            {
            var user = await userManager.FindByNameAsync(loginDTO.Username);
            if (user == null)
                {
                return Unauthorized("Invalid username or password");
                }
            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
                {
                return BadRequest("Invalid username or passworddd");
                }

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>();
            claims.Add(new Claim("username", user.UserName));
            claims.Add(new Claim("userId", user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            foreach (var role in roles)
                {
                claims.Add(new Claim("role", role));
                }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            claims: claims,
            issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            expires: DateTime.Now.AddHours(1),
            signingCredentials: sc);
            var _token = new
                {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                username = loginDTO.Username,
                };
            return Ok(_token);



            }

        }
    }
