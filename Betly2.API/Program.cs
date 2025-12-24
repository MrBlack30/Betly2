using Betly.data; // Ensure this is present to access BetlyContext
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("BetlyDbConnection");

// Register the BetlyContext with the Dependency Injection container
// and configure it to use SQL Server with the connection string.
builder.Services.AddDbContext<BetlyContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories
builder.Services.AddScoped<Betly.core.Interfaces.IUserRepository, Betly.data.Repositories.UserRepository>();
builder.Services.AddScoped<Betly.core.Interfaces.IEventRepository, Betly.data.Repositories.EventRepository>();
builder.Services.AddScoped<Betly.core.Interfaces.IBetRepository, Betly.data.Repositories.BetRepository>();
builder.Services.AddScoped<Betly.core.Interfaces.IFriendRepository, Betly.data.Repositories.FriendRepository>();
builder.Services.AddScoped<Betly.core.Services.BettingService>();

// Add services to the container (Controllers, Swagger, etc.)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// ... rest of the file

var app = builder.Build();

// Apply pending migrations at startup
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<BetlyContext>();
//    await dbContext.Database.MigrateAsync();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();