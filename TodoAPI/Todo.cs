namespace TodoAPI;

public class Todo
{
    public int Id { get; init; }

    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public bool IsComplete { get; set; }
}