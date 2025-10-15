namespace TodoAPI;

public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todo>()
            .Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}