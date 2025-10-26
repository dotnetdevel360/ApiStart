using ApiStart;
using ApiStart.Models;
using ApiStart.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// 애플리케이션 시작 전에 먼저 Serilog 구성
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("애플리케이션 시작");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

    // EF Core SQLite connection (lightweight file DB)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
     ?? "Data Source=localdb.db";
    builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(connectionString));

    // JWT 인증 설정
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key-minimum-32-characters-long-for-production";
    var key = Encoding.ASCII.GetBytes(jwtSecret);
 builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
   x.TokenValidationParameters = new TokenValidationParameters
        {
   ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
     ValidateIssuer = false,
            ValidateAudience = false,
       ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

    // 기본 로깅 완전히 제거하고 Serilog로 교체
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddEndpointsApiExplorer();
 builder.Services.AddSwaggerGen(c =>
    {
        // Swagger에 JWT 인증 옵션 추가
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
 Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
       Scheme = "Bearer",
   BearerFormat = "JWT",
       In = Microsoft.OpenApi.Models.ParameterLocation.Header,
   Description = "JWT Authorization header using the Bearer scheme."
        });
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
      new Microsoft.OpenApi.Models.OpenApiSecurityScheme
              {
       Reference = new Microsoft.OpenApi.Models.OpenApiReference
               {
         Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
       Id = "Bearer"
         }
      },
    new string[] { }
 }
        });
    });

    builder.Services.AddControllers(); // 컨트롤러 서비스 등록 추가
    var app = builder.Build();


 // Apply pending migrations at startup (if any)
    using (var scope = app.Services.CreateScope())
    {
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Apply migrations safely
        try
        {
     if (!db.Database.CanConnect())
            {
     db.Database.EnsureCreated();
            }
            db.Database.Migrate();
        }
        catch (Exception ex)
        {
    Log.Error(ex, "An error occurred while migrating the database.");
   throw;
      }
    }
    

    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapHealthChecks("/health");
    
 app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapControllers(); // 컨트롤러 라우팅 추가
    app.MapEndpoints();
    

    Log.Information("애플리케이션 실행 중");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "애플리케이션 시작 실패");
}
finally
{
    Log.CloseAndFlush();
}


