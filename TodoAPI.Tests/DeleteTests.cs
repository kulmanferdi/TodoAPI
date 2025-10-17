namespace TodoAPI.Tests;

public class DeleteTests
{
    
    private TodoDb GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TodoDb(options);
    }
    
    [Fact]
    public async Task DeleteTodo()
    {
        await using var db = GetInMemoryDb();
        var todo = new Todo { Title = "Write tests", IsComplete = false };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        var todos = await db.Todos.ToListAsync();
        Assert.Empty(todos);

    }
    
[Fact]
    public async Task PostDeleteTodo()
    {
        await using var db = GetInMemoryDb();
        var todo = new Todo { Title = "Write tests", IsComplete = false };
    
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
    
        todo.Title = "Delete soon.";
        await db.SaveChangesAsync();
        
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
    
        var todos = await db.Todos.ToListAsync();
        Assert.Empty(todos);
    }
    
}