using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using DanplannerBooking.Infrastructure.Context;
using DanplannerBooking.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Services
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<DbContextBooking>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICampsiteRepository, CampsiteRepository>();
builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
builder.Services.AddScoped<ICottageRepository, CottageRepository>();
builder.Services.AddScoped<IAddOnRepository, AddOnRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// --------------------
// Auth + JWT
// --------------------
builder.Services.AddAuthorization(options =>
{
    // Så både [Authorize(Roles = "Admin")] og [Authorize(Policy = "AdminOnly")] kan bruges
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});


var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            RoleClaimType = ClaimTypes.Role
        };
    });


// --------------------
// CORS: allow UI origins
// --------------------
var allowedOrigins = new[]
{
    "https://localhost:7090", // UI via Visual Studio (HTTPS)
    "http://localhost:7090",  // UI på HTTP
    "http://localhost:5145"   // evt. ekstra HTTP-profil
};

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Dev", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbContextBooking>();

    // Sikrer at DB + migrations er kørt
    db.Database.Migrate();

    // Hvis der ikke findes nogen admin-bruger endnu, så lav én
    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Hardcoded Admin",
            Email = "admin@admin.com",
            Password = "1234",   // matcher din Login-logik (ingen hashing endnu)
            Phone = "12345678",
            Country = "Denmark",
            Language = "da",
            Role = "Admin"
        };

        db.Users.Add(admin);
        db.SaveChanges();
    }
}


// --------------------
// Middleware pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
