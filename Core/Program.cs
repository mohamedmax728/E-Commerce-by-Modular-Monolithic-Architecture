using Core;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Infrastructre.Context;
using Shared.Infrastructre.Health;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var _configuration = builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var server = Environment.GetEnvironmentVariable("DatabaseServer");
var port = /*Environment.GetEnvironmentVariable("DatabasePort")*/ 1433;
var userId = Environment.GetEnvironmentVariable("DatabaseUser");
var password = Environment.GetEnvironmentVariable("DatabaseUserPassword");
var database = Environment.GetEnvironmentVariable("DatabaseName");
Console.WriteLine($"Server: {server}");
Console.WriteLine($"Port: {port}");
Console.WriteLine($"User ID: {userId}");
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Database: {database}");
//var conString = $"Server={server},{port};Database={database};";
var conString = $"Server={server};Initial Catalog={database};User ID={userId};Password={password};TrustServerCertificate=True;";
// Add services to the container.
//https://localhost:44392/swagger/index.html
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conString));
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(_configuration["ConnectionStrings"]));
DependencyResolver.RegisterServices(builder.Services);
Modules.PaymentProcessing.Api.DependencyInjection.RegisterServices(builder);
builder.Services.AddHealthChecks()
.AddCheck<StripeHealthCheck>("Stripe")
//.AddSqlServer(conString)
.AddSqlServer(_configuration["ConnectionStrings"])
.AddDbContextCheck<AppDbContext>();
var angularOrigin = "Angular";
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:9000");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowCredentials();
        policyBuilder.AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.QueueLimit = 3;
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromSeconds(10);
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                .GetBytes(_configuration["AppSettings:Token"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    using (var scope = (app as IApplicationBuilder).ApplicationServices.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}
app.UseCors(angularOrigin);
app.UseHttpsRedirection();
app.MapHealthChecks("health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
//app.MapPrometheusScrapingEndpoint();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("fixed");
app.Run();
