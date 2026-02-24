using AppSec_Web_API.Data;
using AppSec_Web_API.Domain;
using AppSec_Web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppSec_Web_API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext db, IPasswordHasher hasher, ITokenService tokenService)
        {
            _db = db;
            _hasher = hasher;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                ModelState.AddModelError("email", "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 12)
                ModelState.AddModelError("password", "Password must be at least 12 characters.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var email = request.Email.Trim().ToLowerInvariant();

            var exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                ModelState.AddModelError("email", "Email is already in use.");
                return ValidationProblem(ModelState);
            }

            var user = new User
            {
                Email = email,
                PasswordHash = _hasher.Hash(request.Password),
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                ModelState.AddModelError("email", "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                ModelState.AddModelError("password", "Password is required.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return Unauthorized();

            var ok = _hasher.Verify(request.Password, user.PasswordHash);
            if (!ok)
                return Unauthorized();

            var token = _tokenService.CreateToken(user);

            return Ok(new { accessToken = token, expiresIn = 900 });
        }
    }
}
