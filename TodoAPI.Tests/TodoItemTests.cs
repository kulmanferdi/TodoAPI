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
}
