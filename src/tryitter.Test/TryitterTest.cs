using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using tryitter.Models;
using tryitter.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using tryitter.Test;

using System.Text.Json.Nodes;
using System.Net.Http.Json;

namespace SignInTest.Test;
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

    // Testa as rotas da SignInController
    [Theory(DisplayName = "POST /SignIn com um usuário válido deve retornar status Ok")]
    [MemberData(nameof(rightUserLogin))]
    public async Task SignInShouldReturnOk(UserLogin rightUserLogin)
    {
        var json = JsonConvert.SerializeObject(rightUserLogin);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignIn", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory(DisplayName = "POST /SignIn com um usuário com e-mail e senha inválidos deve retornar Unauthorized")]
    [MemberData(nameof(wrongUserEmail))]
    [MemberData(nameof(wrongUserPassword))]
    public async Task SignInShouldReturnUnauthorized(UserLogin wrongUser)
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


    // Testa as rotas da SignUpController
    [Theory(DisplayName = "POST /SignUp com um usuário válido deve retornar status Created")]
    [MemberData(nameof(rightNewUser))]
    public async Task SignUpShouldReturnCreated(User rightNewUser)
    {
        var json = JsonConvert.SerializeObject(rightNewUser);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignUp", content);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory(DisplayName = "POST /SignUp com um usuário com e-mail ou nome já cadastrados deve retornar status BadRequest")]
    [MemberData(nameof(repeatedUserName))]
    [MemberData(nameof(repeatedUserEmail))]
    public async Task SignUpShouldReturnBadRequest(User wrongUser)
    {
        var json = JsonConvert.SerializeObject(wrongUser);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignUp", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public static readonly TheoryData<User> rightNewUser = new()
    {
            new User {
                Name = "Aluno da Trybe",
                Email = "aluno@email.com",
                Module = "Back-end",
                Status = "Concluído",
                Password = "aluno.123",
            },
    };

    public static readonly TheoryData<User> repeatedUserName = new()
    {
      new User { Name = "User 01", Email = "new@email.com", Module = "Front-end", Status = "Concluído", Password = "one.123"}
    };

    public static readonly TheoryData<User> repeatedUserEmail = new()
    {
      new User { Name = "User 10", Email = "one@email.com", Module = "Front-end", Status = "Concluído", Password = "one.123"}
    };
}

