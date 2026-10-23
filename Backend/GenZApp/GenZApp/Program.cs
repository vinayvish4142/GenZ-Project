
using GenZApp.Models;
using GenZApp.Repository.Student;
using GenZApp.Services;
using GenZApp.Services.Student;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositorys.Authenticate;
using SchoolManagement.Infrastructure.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ======================================
// 1. Controllers and Swagger
// ======================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================
// 2. SQL Server Database Connection
// ======================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ======================================
// 3. ASP.NET Core Identity
// ======================================

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ======================================
// 4. JWT Configuration
// ======================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must be at least 32 bytes.");
}

// ======================================
// 5. JWT Authentication
// ======================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                RoleClaimType = ClaimTypes.Role,
                NameClaimType = "sub",

                ClockSkew = TimeSpan.Zero
            };
    });

// ======================================
// 6. Authorization
// ======================================

builder.Services.AddAuthorization();

// ======================================
// 7. Dependency Injection
// ======================================

builder.Services.AddScoped<
    IRefreshTokenRepository,
    RefreshTokenRepository>();

builder.Services.AddScoped<
    ITokenService,
    JwtTokenService>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

// Uncomment when implemented:
// builder.Services.AddScoped<IStudentService, StudentService>();

// ======================================
// 8. Angular CORS Configuration
// ======================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================================
// 9. Build Application
// ======================================

var app = builder.Build();

// ======================================
// 10. Swagger Middleware
// ======================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ======================================
// 11. Middleware Pipeline
// ======================================

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AngularPolicy");

app.UseAuthentication();

app.UseAuthorization();

// ======================================
// 12. Map API Controllers
// ======================================

app.MapControllers();

// ======================================
// 13. Default Root Endpoint (404 Fix)
// ======================================

app.MapGet("/", () => Results.Ok(new
{
    Status = "Success",
    Message = "GenZ API is running successfully",
    Version = "1.0",
    Swagger = "/swagger"
}))
.AllowAnonymous();

// ======================================
// 14. Health Check Endpoint
// ======================================

app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Application = "GenZ API",
    Timestamp = DateTime.UtcNow
}))
.AllowAnonymous();

// ======================================
// 15. Seed Identity Roles
// ======================================

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedAsync(roleManager);
}

// ======================================
// 16. Run Application
// ======================================

app.Run();
