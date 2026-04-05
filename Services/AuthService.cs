using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

using ToDoList.Api.DTOs.Auth;
using ToDoList.Api.Models;
using ToDoList.Api.Repositories.Interfaces;

using ToDoList.Api.Services.Interfaces;

namespace ToDoList.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;
    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }


    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // UNIQ TEST
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        throw new InvalidOperationException("Email already in use.");

        if (await _userRepository.ExistsByEmailAsync(dto.UserName))
        throw new InvalidOperationException("UserName already in use.");

        var user = new User
        {
            Username = dto.UserName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return GenerateToken(user);
    }


    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");
        
        if(!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return GenerateToken(user);
    }

    // -------------------------------------------------------------

    private AuthResponseDto GenerateToken(User user)
    {
        var jwtKey      = _config["Jwt:Key"]!;
        var jwtIssuer   = _config["Jwt:Issuer"]!;
        var expiresAt   = DateTime.UtcNow.AddHours(
            double.Parse(_config["Jwt:ExpiresInHours"] ?? "24"));
    
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,    user.Id.ToString()),
            new Claim(ClaimTypes.Name,              user.Username),
            new Claim(ClaimTypes.Email,             user.Email)

        };

        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token   = new JwtSecurityToken(
            issuer:             jwtIssuer,
            audience:           jwtIssuer,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = user.Username,
            ExpiresAt = expiresAt
        };
    }
}