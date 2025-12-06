namespace TodoAPI;

[Table("Todos")]
public class Todo
{
    public int Id { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public bool IsComplete { get; set; }
}