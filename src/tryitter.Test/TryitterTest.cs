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

// dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=lcov

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


    [Theory(DisplayName = "POST /Sign Deve retornar um JWT token")]
    [MemberData(nameof(userLogin))]
    public async Task ShouldReturnAJWTToken(UserLogin validUser)
    {
        var json = JsonConvert.SerializeObject(validUser);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("SignIn", content);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    public static readonly TheoryData<UserLogin> userLogin = new()
    {
            new UserLogin {
                Email = "one@email.com",
                Password = "one.123"
            },
    };
}

