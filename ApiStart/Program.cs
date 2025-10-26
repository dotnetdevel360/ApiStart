using ApiStart;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

// 애플리케이션 시작 전에 먼저 Serilog 구성
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("애플리케이션 시작");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHealthChecks()
 .AddCheck("self", () => HealthCheckResult.Healthy());

    // EF Core SQLite connection (lightweight file DB)
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
     ?? "Data Source=localdb.db";
    builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(connectionString));

    // 기본 로깅 완전히 제거하고 Serilog로 교체
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers(); // 컨트롤러 서비스 등록 추가
    var app = builder.Build();


    // Apply pending migrations at startup (if any)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Apply migrations safely
        try
        {
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

