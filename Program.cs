using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Microsoft.AspNetCore.Mvc;

using ToDoList.Api.DTOs.Common;

using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;

using ToDoList.Api.Repositories;
using ToDoList.Api.Repositories.Interfaces;

using ToDoList.Api.Services;
using ToDoList.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Repositories ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// --- Services ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITodoService, TodoService>();

// --- JWT ---
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer              = true,
            ValidateAudience            = true,
            ValidateLifetime            = true,
            ValidateIssuerSigningKey    = true,

            ValidIssuer                 = builder.Configuration["Jwt:Issuer"],
            ValidAudience               = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey            = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthentication();

// Swagger
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Собираем ошибки в словарь: { "Title": ["поле обязательно"] }
            var details = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors
                                     .Select(e => e.ErrorMessage)
                                     .ToArray()
                );

            var response = ApiResponse<object>.Fail(
                ErrorCodes.ValidationError,
                "One or more validation errors occurred.",
                details
            );

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Добавляем кнопку Authorize в Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Введи JWT токен. Пример: Bearer {token}"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// AUTO MIGRATION
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();