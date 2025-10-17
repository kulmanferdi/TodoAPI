namespace TodoAPI.Tests;

[Collection("Database collection")]
public class InMemoryTests
{
    private TodoDb GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TodoDb(options);
    }

    [Fact]
    public async Task AddTodo_ShouldSaveToDatabase()
    {
        await using var db = GetInMemoryDb();
        var todo = new Todo { Title = "Write tests", IsComplete = false };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        var todos = await db.Todos.ToListAsync();
        Assert.Single(todos);
        Assert.Equal("Write tests", todos.First().Title);
        Assert.False(todos.First().IsComplete);
    }

    [Fact]
    public async Task MarkTodoDone_ShouldUpdateField()
    {
        await using var db = GetInMemoryDb();
        var todo = new Todo { Title = "Check EF Core", IsComplete = false };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        todo.IsComplete = true;
        db.Todos.Update(todo);
        await db.SaveChangesAsync();

        var updated = await db.Todos.FindAsync(todo.Id);
        Assert.True(updated!.IsComplete);
    }
}