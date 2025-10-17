namespace TodoAPI.Tests;

[Collection("Database collection")]
public class TodoDbSqliteTests
{
    private TodoDb CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<TodoDb>()
            .UseSqlite(connection)
            .Options;
        return new TodoDb(options);
    }

    [Fact]
    public async Task AddTodo_ShouldSaveInDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using (var context = CreateContext(connection))
        {
            await context.Database.EnsureCreatedAsync();

            var todo = new Todo { Title = "SQLite test", IsComplete = false };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(connection))
        {
            var todos = await context.Todos.ToListAsync();
            Assert.Single(todos);
            Assert.Equal("SQLite test", todos.First().Title);
            Assert.False(todos.First().IsComplete);
        }

        await connection.CloseAsync();
    }

    [Fact]
    public async Task UpdateTodo_ShouldSetIsDoneTrue()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        await using (var context = CreateContext(connection))
        {
            await context.Database.EnsureCreatedAsync();

            var todo = new Todo { Title = "Complete update test", IsComplete = false };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();

            todo.IsComplete = true;
            context.Todos.Update(todo);
            await context.SaveChangesAsync();
        }

        await using (var context = CreateContext(connection))
        {
            var todo = await context.Todos.FirstAsync();
            Assert.True(todo.IsComplete);
        }

        await connection.CloseAsync();
    }
}