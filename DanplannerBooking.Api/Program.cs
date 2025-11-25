using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Infrastructure.Context;
using DanplannerBooking.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    // Politik hvis du vil bruge [Authorize(Policy = "AdminOnly")]
    options.AddPolicy("AdminOnly", p => p.RequireClaim("IsAdmin", "true"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();   // <-- BESKYT MOD ANONYMOUS
        policy.RequireRole("Admin");         // <-- KR�V ADMIN
    });
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
            ClockSkew = TimeSpan.Zero
        };
    });

// --------------------
// CORS: allow UI origins
// --------------------
var allowedOrigins = new[]
{
    "https://localhost:7090", // UI via Visual Studio (HTTPS)
    "http://localhost:7090",  // UI p� HTTP
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
