using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Tests
{
    public class TodoItemTestContextSetup
    {

        public TodoItemTestContextSetup(DbContextOptions<TodoContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<TodoContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new TodoContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var fst = new TodoItem
                {
                    Id = 1,
                    Name = "Walk dog",
                    IsComplete = false,
                };

                var snd = new TodoItem
                {
                    Id = 2,
                    Name = "Walk snd dog",
                    IsComplete = true,
                };

                var trd = new TodoItem
                {
                    Id = 3,
                    Name = "Walk trd dog",
                    IsComplete = false,
                };

                context.AddRange(fst, snd, trd);

                context.SaveChanges();
            }
        }

    }
}
