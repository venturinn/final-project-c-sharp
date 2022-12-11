using tryitter.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// Configuração para usar o token JWT com a interface do Swagger:
// REF.: https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Projeto Tryitter",
        Description = "Desafio final da Acelaração 'C# - do zero ao deploy' - Trybe/XPInc",
        Contact = new OpenApiContact() { Name = "Diego Venturin", Email = "diegoventurinn@gmail.com" }
    });
    
    setup.UseInlineDefinitionsForEnums();
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});


builder.Services.AddDbContext<TryitterContext>();
//builder.Services.AddScoped<ITryitterContext, TryitterContext>();
//builder.Services.AddScoped<ITryitterRepository, TryitterRepository>();
builder.Services.AddScoped<TryitterRepository>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SecretSecretSecretSecret")) // Alterar isso aqui!
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdmLogin", policy => policy.RequireClaim("AdmLogin"));
    options.AddPolicy("UserLogin", policy => policy.RequireClaim("UserLogin"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
