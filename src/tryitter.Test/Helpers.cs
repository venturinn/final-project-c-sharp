using Microsoft.EntityFrameworkCore;
using tryitter.Models;
using tryitter.Repository;

namespace tryitter.Test
{
    public static class Helpers
    {
        public static TryitterContext GetContextInstanceForTests(string inMemoryDbName)
        {
            var contextOptions = new DbContextOptionsBuilder<TryitterContext>()
                .UseInMemoryDatabase(inMemoryDbName)
                .Options;
            var context = new TryitterContext(contextOptions);

            context.Users.AddRange(
                GetUsersListForTests()
            );

            context.Posts.AddRange(
                GetPostsListForTests()
            );

            context.SaveChanges();
            return context;
        }

        public static List<Post> GetPostsListForTests() =>
            new() {
                new Post {
                    PostId = 1,
                    Content = "Primeiro post do usuário 01",
                    UserId = 2,
                },
                  new Post {
                    PostId = 2,
                    Content = "Segundo post do usuário 01",
                    UserId = 2,
                },
                  new Post {
                    PostId = 3,
                    Content = "Terceiro post do usuário 01",
                    UserId = 2,
                },
                new Post {
                    PostId = 4,
                    Content = "Primeiro post do usuário 03",
                    UserId = 3,
                },
                new Post {
                    PostId = 5,
                    Content = "Segundo post do usuário 03",
                    UserId = 3,
                },
            };

        public static List<User> GetUsersListForTests() =>
            new() {
                new User { UserId  = 1, Name = "adm", Email = "adm@email.com", Module = "NA", Status = "NA", Password = "adm.123"},
                new User { UserId  = 2, Name = "User 01", Email = "one@email.com", Module = "Front-end", Status = "Concluído", Password = "one.123"},
                new User { UserId  = 3, Name = "User 02", Email = "two@email.com", Module = "Back-end", Status = "Cursando", Password = "two.123"}
            };
    }
}
