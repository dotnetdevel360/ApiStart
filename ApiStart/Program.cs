using ApiStart;
using Microsoft.AspNetCore.Builder;
using Serilog;

// 애플리케이션 시작 전에 먼저 Serilog 구성
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("애플리케이션 시작");

    var builder = WebApplication.CreateBuilder(args);

    // 기본 로깅 완전히 제거하고 Serilog로 교체
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
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

