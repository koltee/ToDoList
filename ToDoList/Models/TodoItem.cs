using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models;

public enum TodoStatus
{
    Pendente = 0,
    Fazendo = 1,
    Concluido = 2
}

public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.Pendente;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
