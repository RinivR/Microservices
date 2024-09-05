namespace MyFirstMicroservice.DatabaseContext
{
    using Microsoft.EntityFrameworkCore;
    using MyFirstMicroservice.Model;

    public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
    {
        public DbSet<Todo> Todos => Set<Todo>();

    }
}
