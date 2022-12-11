using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using tryitter.Models;
using tryitter.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Newtonsoft.Json;
using System.Text;

using System.Text.Json.Nodes;
using System.Net.Http.Json;

namespace tryitter.Test;
public class TryitterTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient client;

    public TryitterTest(WebApplicationFactory<Program> factory)
    {
        client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TryitterContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<TryitterContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTest");
                });
                services.AddScoped<ITryitterContext, TryitterContext>();
                services.AddScoped<ITryitterRepository, TryitterRepository>();

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<TryitterContext>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();

                    appContext.Users.AddRange(
                        Helpers.GetUsersListForTests()
                    );

                    appContext.Posts.AddRange(
                        Helpers.GetPostsListForTests()
                    );

                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }


    [Theory(DisplayName = "POST /Sign com um usuário válido deve retornar status Ok")]
    [MemberData(nameof(rightUserLogin))]
    public async Task ShouldReturnAJWTToken(UserLogin rightUserLogin)
    {
        var json = JsonConvert.SerializeObject(rightUserLogin);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignIn", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(DisplayName = "POST /Sign com um usuário com e-mail e senha inválidos deve retornar Unauthorized")]
    [MemberData(nameof(wrongUserEmail))]
    [MemberData(nameof(wrongUserPassword))]
    public async Task ShouldReturnUnauthorized(UserLogin wrongUser)
    {
        var json = JsonConvert.SerializeObject(wrongUser);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignIn", content);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public static readonly TheoryData<UserLogin> rightUserLogin = new()
    {
            new UserLogin {
                Email = "one@email.com",
                Password = "one.123"
            },
    };

    public static readonly TheoryData<UserLogin> wrongUserPassword = new()
    {
            new UserLogin {
                Email = "one@email.com",
                Password = "wrongPassword"
            },
    };

    public static readonly TheoryData<UserLogin> wrongUserEmail = new()
    {
            new UserLogin {
                Email = "wrongEmail",
                Password = "one.123"
            },
    };
}

