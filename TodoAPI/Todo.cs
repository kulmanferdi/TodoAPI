namespace TodoAPI;

public class Todo
{
    public int Id { get; set; }
    
    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public bool IsComplete { get; set; }
}