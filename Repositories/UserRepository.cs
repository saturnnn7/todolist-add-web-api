using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Models;
using ToDoList.Api.Repositories.Interfaces;

namespace ToDoList.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;


    public async Task<User?> GetByIdAsync(Guid id)
    => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email)
    => await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<bool> ExistsByEmailAsync(string email)
    => await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());

    public async Task<bool> ExistsByUsernameAsync(string username)
    => await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());

    public async Task AddAsync(User user)
    => await _context.Users.AddAsync(user);

    public async Task SaveChangesAsync()
    => await _context.SaveChangesAsync();
}