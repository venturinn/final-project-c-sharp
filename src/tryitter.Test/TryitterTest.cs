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
using tryitter.Services;
using System.Net.Http.Headers;

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

    // Testa a rota da SignInController
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


    // Testa a rota da SignUpController
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


    // Testa as rotas da UserLoginController
    [Theory(DisplayName = "GET /UserLogin/allmyposts deve retornar todos os posts do usuário logado")]
    [MemberData(nameof(UserLoginShouldReturnAllPostsEntries))]
    public async Task UserLoginShouldReturnAllPosts(UserLogin rightUserLogin, List<PostDTO> expectedPosts)
    {
        // Realizando o login para receber JWT Token
        var json = JsonConvert.SerializeObject(rightUserLogin);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var responseSignIn = await client.PostAsync("SignIn", content);
        var token = await responseSignIn.Content.ReadFromJsonAsync<Token>();

        // Requisição para o endpoint a ser testado
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenMessage);
        var response = await client.GetAsync("UserLogin/allmyposts");

        var posts = await response.Content.ReadFromJsonAsync<List<PostDTO>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        posts.Should().BeEquivalentTo(expectedPosts);
    }

    [Theory(DisplayName = "GET /UserLogin/allmyposts deve retornar NotFound quando o usuário não possuir posts")]
    [MemberData(nameof(UserWithoutPosts))]
    public async Task UserLoginShouldReturnNotFound(UserLogin rightUserLogin)
    {
        // Realizando o login para receber JWT Token
        var json = JsonConvert.SerializeObject(rightUserLogin);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var responseSignIn = await client.PostAsync("SignIn", content);
        var token = await responseSignIn.Content.ReadFromJsonAsync<Token>();

        // Requisição para o endpoint a ser testado
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenMessage);
        var response = await client.GetAsync("UserLogin/allmyposts");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory(DisplayName = "GET /UserLogin/mylastpost deve retornar o último post do usuário logado")]
    [MemberData(nameof(UserLoginShouldReturnLastPostEntries))]
    public async Task UserLoginShouldReturnLastPost(UserLogin rightUserLogin, PostDTO expectedPost)
    {
        // Realizando o login para receber JWT Token
        var json = JsonConvert.SerializeObject(rightUserLogin);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        var responseSignIn = await client.PostAsync("SignIn", content);
        var token = await responseSignIn.Content.ReadFromJsonAsync<Token>();

        // Requisição para o endpoint a ser testado
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenMessage);
        var response = await client.GetAsync("UserLogin/mylastpost");

        var post = await response.Content.ReadFromJsonAsync<PostDTO>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        post.Should().BeEquivalentTo(expectedPost);
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

    public static readonly TheoryData<UserLogin, List<PostDTO>> UserLoginShouldReturnAllPostsEntries = new()
    {
        {
            new UserLogin { Email = "one@email.com",Password = "one.123"},
            new List<PostDTO>() {
                new PostDTO { PostId = 1, Content = "Primeiro post do usuário 02", UserId = 2},
                new PostDTO { PostId = 2, Content = "Segundo post do usuário 02", UserId = 2},
                new PostDTO { PostId = 3, Content = "Terceiro post do usuário 02", UserId = 2},
            }
        }
    };

    public static readonly TheoryData<UserLogin> UserWithoutPosts = new()
    {
            new UserLogin {
                Email = "three@email.com",
                Password = "three.123"
            },
    };

    public static readonly TheoryData<UserLogin, PostDTO> UserLoginShouldReturnLastPostEntries = new()
    {
        {
            new UserLogin { Email = "one@email.com",Password = "one.123"},
            new PostDTO { PostId = 3, Content = "Terceiro post do usuário 02", UserId = 2}
        }
    };
}

