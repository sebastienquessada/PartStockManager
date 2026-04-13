using Microsoft.EntityFrameworkCore;
using PartStockManager.Adapter.Database.Context;
using PartStockManager.Adapter.Repositories;
using PartStockManager.CoreLogic.Repositories;
using Serilog;

// Why a try/catch ?  If the API crashes when starting (bad config, port already used or inaccessible), we can have a trace of what is wrong
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Logger initialization
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(Path.Combine("logs", "PartStockManagerApp-.txt"), rollingInterval: RollingInterval.Day)
        .CreateLogger();

    // Allows to use the logger in the repositories and the API
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // Get the connexion string
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Verify the injection configuration
    builder.Services.AddDbContext<StockDbContext>(options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IStockRepository, StockRepository>();
    builder.Services.AddScoped<IPartRepository, PartRepository>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Part Stock Manager API v1");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    // Optional: Allows to log automatically each incoming HTTP request (useful to debug the API)
    app.UseSerilogRequestLogging();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (
    // Ignore the exception if it was triggered by the migration tool
    ex.GetType().Name is not "StopTheHostException" &&
    ex.GetType().Name is not "HostAbortedException")
{
    Log.Fatal(ex, "The application crashed");
}
finally
{
    Log.CloseAndFlush(); // Empty the Serilog buffer of logs files before stopping the app
}

// To explain the line below, since version 10 of C#, we can write the "Program.cs" file without declaring the usual structure "class Program { static void Main() }".
// However, when this structure is automatically created by the compiler, the "Programm" class is created as a internal class:
// the public partial class allows us to fuse the generated class with this one
public partial class Program { }