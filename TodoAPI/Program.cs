var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt =>
    opt.UseSqlite("Data Source=Database/todos.db"));

var app = builder.Build();

app.MapGet("/todos", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todos/completed", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todos/incomplete", async (TodoDb db) =>
    await db.Todos.Where(t => !t.IsComplete).ToListAsync());

app.MapGet("/todos/count", async (TodoDb db) =>
    await db.Todos.CountAsync());

app.MapGet("/todos/count/completed", async (TodoDb db) =>
    await db.Todos.CountAsync(t => t.IsComplete));

app.MapGet("/todos/count/incomplete", async (TodoDb db) =>
    await db.Todos.CountAsync(t => !t.IsComplete));

app.MapGet("/todos/search/{term}", async (string term, TodoDb db) =>
    await db.Todos
        .Where(t => t.Title.Contains(term))
        .ToListAsync());

app.MapGet("/todos/ordered/asc", async (TodoDb db) =>
    await db.Todos.OrderBy(t => t.Title).ToListAsync());

app.MapGet("/todos/ordered/desc", async (TodoDb db) =>
    await db.Todos
        .OrderByDescending(t => t.Title)
        .ToListAsync());

app.MapGet("/todos/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapPost("/todos", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = inputTodo.Title;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPatch("/todos/complete/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.IsComplete = true;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

app.MapDelete("/todos/completed", async (TodoDb db) =>
{
    var completedTodos = db.Todos.Where(t => t.IsComplete);
    db.Todos.RemoveRange(completedTodos);
    var deletedCount = await db.SaveChangesAsync();
    return Results.Ok(new { Deleted = deletedCount });
});

app.Run();