using Microsoft.AspNetCore.Identity;
using UserMicroservice.Data.DTOs;
using UserMicroservice.Data.Entities;
using UserMicroservice.Data;
using UserMicroservice.Services.JwtService;
using Microsoft.EntityFrameworkCore;

namespace UserMicroservice.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtService.JwtService _jwtService;

        public UserService(UserDbContext context, IPasswordHasher<User> passwordHasher, JwtService.JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<UserDto> RegisterAsync(RegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            if (dto.Image != null)
            {
                var filePath = Path.Combine("Uploads", $"{Guid.NewGuid()}_{dto.Image.FileName}");
                Directory.CreateDirectory("Uploads");
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);
                user.ImageUrl = filePath;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            };
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            return _jwtService.GenerateToken(user);
        }
    }
}
