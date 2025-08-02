using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        });

        // TodoItem configuration
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

            // Foreign key relationship
            entity.HasOne(e => e.User)
                  .WithMany(u => u.TodoItems)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index for better query performance
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
        });
    }
}
