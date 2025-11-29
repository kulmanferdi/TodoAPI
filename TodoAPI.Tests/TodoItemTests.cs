namespace TodoAPI.Tests;

public class TodoItemTests
{
    [Fact]
    public void TodoItem_StoresPropertiesCorrectly()
    {
        var todo = new Todo { Id = 1, Title = "Buy milk" };
        Assert.Equal(1, todo.Id);
        Assert.Equal("Buy milk", todo.Title);
    }

    [Fact]
    public void TodoItem_MarkAsComplete_Works()
    {
        var todo = new Todo
        {
            Id = 1, Title = "Play guitar",
            IsComplete = false
        };
        Assert.False(todo.IsComplete);
        todo.IsComplete = true;
        Assert.True(todo.IsComplete);
    }
}
