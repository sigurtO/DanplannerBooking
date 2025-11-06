using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Infrastructure.Context;
using DanplannerBooking.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContextBooking>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireClaim("IsAdmin", "true"));
});

// CORS: allow UI origins
var allowedOrigins = new[]
{
    "https://localhost:7090", // UI via Visual Studio (HTTPS)
    "http://localhost:5145"   // UI via `dotnet run` (HTTP)
};

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Dev", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Dev");
app.UseAuthorization();
app.MapControllers();
app.Run();
