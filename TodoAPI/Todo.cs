namespace TodoAPI;

public class Todo
{
    public int Id { get; set; }
    
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public bool IsComplete { get; set; }
}