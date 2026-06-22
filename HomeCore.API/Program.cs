using HomeCore.BLL;
using HomeCore.BLL.Monitors;
using HomeCore.DAL;
using HomeCore.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresá el token JWT así: Bearer {token}"
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

//JWT Auth
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key debe configurarse en user-secrets.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

//Persistence
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=homecore.db";

builder.Services.AddSingleton<IDbConnectionFactory>(
    new SqliteConnectionFactory(connectionString));

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Auth Service
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Plugin system
builder.Services.AddHttpClient<HttpHealthCheckMonitor>();
builder.Services.AddScoped<IServiceMonitor, HttpHealthCheckMonitor>();
builder.Services.AddScoped<ServiceMonitorFactory>();

//Build and run the app
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    var adminUserName = app.Configuration["Admin:UserName"]
        ?? throw new InvalidOperationException("Admin:UserName debe configurarse en appsettings.");
    var adminPassword = app.Configuration["Admin:Password"]
        ?? throw new InvalidOperationException("Admin:Password debe configurarse en user-secrets.");

    var tempUser = new User { UserName = adminUserName };
    var adminPasswordHash = passwordHasher.HashPassword(tempUser, adminPassword);

    dbInitializer.Initialize(adminUserName, adminPasswordHash);
}

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