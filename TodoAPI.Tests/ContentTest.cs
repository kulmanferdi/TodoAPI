namespace TodoAPI.Tests;

public class ContentTest
{
    [Fact]
    public Task EmptyTitleTest()
    {
        try
        {
            var todo = new Todo { Title = "", IsComplete = false };
            Assert.Equal("", todo.Title);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    [Fact]
    public Task TitleTest()
    {
        try
        {
            var todo = new Todo { Title = "Foo", IsComplete = false };
            Assert.Equal("Foo", todo.Title);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    [Fact]
    public Task ModifyTitleTest()
    {
        try
        {
            var todo = new Todo { Title = "Foo", IsComplete = false };
            Assert.Equal("Foo", todo.Title);
            todo.Title = "Bar";
            Assert.Equal("Bar", todo.Title);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    [Fact]
    public async Task ClearTitleTest()
    {
        try
        {
            var todo = new Todo { Title = "Foo", IsComplete = false };
            Assert.Equal("Foo", todo.Title);
            todo.Title = "";
            Assert.Equal("", todo.Title);
        }
        catch (Exception exception)
        {
            await Task.FromException(exception);
        }
    }

    [Fact]
    public async Task NullTitleTest()
    {
        try
        {
            var todo = new Todo { Title = null, IsComplete = false };
            Assert.Null(todo.Title);
        }
        catch (Exception exception)
        {
            await Task.FromException(exception);
        }
    }
}