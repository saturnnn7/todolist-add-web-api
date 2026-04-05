using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Models;
using ToDoList.Api.Models.Enums;

namespace ToDoList.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasIndex(u => u.Email)
                    .IsUnique();

            entity.HasIndex(u => u.Username)
                    .IsUnique();

            entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

            entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

            entity.Property(u => u.PasswordHash)
                    .IsRequired();
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                    .IsRequired()
                    .HasMaxLength(200);

            entity.Property(t => t.Description)
                    .HasMaxLength(2000);

            entity.Property(t => t.Tags)
                    .HasMaxLength(500);

            entity.Property(t => t.Tags)
                    .HasConversion<string>();

            entity.HasOne(t => t.User)
                    .WithMany(u => u.Todos)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        });
    }
}