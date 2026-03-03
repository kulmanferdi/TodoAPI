using Microsoft.AspNetCore.Hosting;

namespace TodoAPI.Tests;

public class TodoApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TodoDb>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<TodoDb>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        _connection.Dispose();
        base.Dispose(disposing);
    }
}

public class ApiIntegrationTests : IClassFixture<TodoApiFactory>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(TodoApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Todo> CreateTodoAsync(string title, bool isComplete = false)
    {
        var response = await _client.PostAsJsonAsync("/todos", new { Title = title, IsComplete = isComplete });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Todo>())!;
    }

    [Fact]
    public async Task GetTodos_ReturnsOkWithList()
    {
        var response = await _client.GetAsync("/todos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
    }

    [Fact]
    public async Task PostTodo_ReturnsCreatedWithTodo()
    {
        var response = await _client.PostAsJsonAsync("/todos", new { Title = "Post test", IsComplete = false });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(created);
        Assert.Equal("Post test", created.Title);
        Assert.False(created.IsComplete);

        await _client.DeleteAsync($"/todos/{created.Id}");
    }

    [Fact]
    public async Task GetTodoById_ExistingId_ReturnsOk()
    {
        var created = await CreateTodoAsync("GetById test");

        var response = await _client.GetAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(todo);
        Assert.Equal("GetById test", todo.Title);

        await _client.DeleteAsync($"/todos/{created.Id}");
    }

    [Fact]
    public async Task GetTodoById_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/todos/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutTodo_ExistingId_ReturnsNoContent()
    {
        var created = await CreateTodoAsync("Put original");

        var response = await _client.PutAsJsonAsync($"/todos/{created.Id}", new { Title = "Put updated", IsComplete = true });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");
        var updated = await getResponse.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(updated);
        Assert.Equal("Put updated", updated.Title);
        Assert.True(updated.IsComplete);

        await _client.DeleteAsync($"/todos/{created.Id}");
    }

    [Fact]
    public async Task PutTodo_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/todos/999999", new { Title = "Ghost", IsComplete = false });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchComplete_ExistingId_ReturnsTodoMarkedComplete()
    {
        var created = await CreateTodoAsync("Patch test", isComplete: false);

        var response = await _client.PatchAsync($"/todos/complete/{created.Id}", new StringContent(""));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        Assert.NotNull(todo);
        Assert.True(todo.IsComplete);

        await _client.DeleteAsync($"/todos/{created.Id}");
    }

    [Fact]
    public async Task PatchComplete_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.PatchAsync("/todos/complete/999999", new StringContent(""));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_ExistingId_ReturnsOkAndRemovesItem()
    {
        var created = await CreateTodoAsync("Delete test");

        var deleteResponse = await _client.DeleteAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_NonExistingId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/todos/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCompletedTodos_ReturnsOnlyCompletedItems()
    {
        var completed = await CreateTodoAsync("Completed item", isComplete: true);
        var incomplete = await CreateTodoAsync("Incomplete item", isComplete: false);

        var response = await _client.GetAsync("/todos/completed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
        Assert.All(todos, t => Assert.True(t.IsComplete));

        await _client.DeleteAsync($"/todos/{completed.Id}");
        await _client.DeleteAsync($"/todos/{incomplete.Id}");
    }

    [Fact]
    public async Task GetIncompleteTodos_ReturnsOnlyIncompleteItems()
    {
        var completed = await CreateTodoAsync("Done item", isComplete: true);
        var incomplete = await CreateTodoAsync("Todo item", isComplete: false);

        var response = await _client.GetAsync("/todos/incomplete");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
        Assert.All(todos, t => Assert.False(t.IsComplete));

        await _client.DeleteAsync($"/todos/{completed.Id}");
        await _client.DeleteAsync($"/todos/{incomplete.Id}");
    }

    [Fact]
    public async Task GetTodosCount_ReturnsNonNegativeInteger()
    {
        var response = await _client.GetAsync("/todos/count");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(count >= 0);
    }

    [Fact]
    public async Task GetCompletedCount_ReturnsNonNegativeInteger()
    {
        var response = await _client.GetAsync("/todos/count/completed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(count >= 0);
    }

    [Fact]
    public async Task GetIncompleteCount_ReturnsNonNegativeInteger()
    {
        var response = await _client.GetAsync("/todos/count/incomplete");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(count >= 0);
    }

    [Fact]
    public async Task SearchTodos_ReturnsMatchingResults()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];
        var created = await CreateTodoAsync($"Search {unique} item");

        var response = await _client.GetAsync($"/todos/search/{unique}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
        Assert.Contains(todos, t => t.Title.Contains(unique));

        await _client.DeleteAsync($"/todos/{created.Id}");
    }

    [Fact]
    public async Task GetOrderedAsc_ReturnsTodosInAscendingOrder()
    {
        var a = await CreateTodoAsync("Asc Apple");
        var b = await CreateTodoAsync("Asc Banana");
        var c = await CreateTodoAsync("Asc Cherry");

        var response = await _client.GetAsync("/todos/ordered/asc");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
        var titles = todos.Select(t => t.Title).ToList();
        Assert.Equal(titles.OrderBy(t => t).ToList(), titles);

        await _client.DeleteAsync($"/todos/{a.Id}");
        await _client.DeleteAsync($"/todos/{b.Id}");
        await _client.DeleteAsync($"/todos/{c.Id}");
    }

    [Fact]
    public async Task GetOrderedDesc_ReturnsTodosInDescendingOrder()
    {
        var a = await CreateTodoAsync("Desc Apple");
        var b = await CreateTodoAsync("Desc Banana");
        var c = await CreateTodoAsync("Desc Cherry");

        var response = await _client.GetAsync("/todos/ordered/desc");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(todos);
        var titles = todos.Select(t => t.Title).ToList();
        Assert.Equal(titles.OrderByDescending(t => t).ToList(), titles);

        await _client.DeleteAsync($"/todos/{a.Id}");
        await _client.DeleteAsync($"/todos/{b.Id}");
        await _client.DeleteAsync($"/todos/{c.Id}");
    }

    [Fact]
    public async Task DeleteCompletedTodos_RemovesAllCompletedAndReturnsCount()
    {
        var c1 = await CreateTodoAsync("Batch delete 1", isComplete: true);
        var c2 = await CreateTodoAsync("Batch delete 2", isComplete: true);

        var response = await _client.DeleteAsync("/todos/completed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var completedResponse = await _client.GetAsync("/todos/completed");
        var remaining = await completedResponse.Content.ReadFromJsonAsync<List<Todo>>();
        Assert.NotNull(remaining);
        Assert.Empty(remaining);
    }
}
