using Betly.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. הגדרת מסד הנתונים
var connectionString = builder.Configuration.GetConnectionString("BetlyDbConnection");
builder.Services.AddDbContext<BetlyContext>(options =>
    options.UseSqlServer(connectionString));

// 2. חיבור ה-Repository
builder.Services.AddScoped<Betly.core.Interfaces.IUserRepository, Betly.data.Repositories.UserRepository>();

// 3. הגדרת מנגנון האימות (JWT Authentication)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הגדרות סביבת פיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- סדר חשוב מאוד! ---
app.UseAuthentication(); // קודם בודקים מי המשתמש
app.UseAuthorization();  // אחר כך בודקים מה מותר לו לעשות

app.MapControllers();

app.Run();