using Core;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Infrastructre.Context;
using Shared.Infrastructre.Health;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var _configuration = builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

// Add services to the container.
//https://localhost:44392/swagger/index.html
//builder.Services.AddOrchardCore();
//var assembly = Assembly.Load("Modules.OrderManagement.Api");
//var externalController = new AssemblyPart(assembly);
builder.Services.AddDbContext<AppDbContext>();
DependencyResolver.RegisterServices(builder.Services);
Modules.CustomerManagement.Api.DependencyInjection.RegisterServices(builder.Services);
Modules.PaymentProcessing.Api.DependencyInjection.RegisterServices(builder);
Modules.ProductCatalog.Api.DependencyInjection.RegisterServices(builder.Services);
Modules.ShoppingCart.Api.DependencyInjection.RegisterServices(builder.Services);
Modules.OrderManagement.Api.DependencyInjection.RegisterServices(builder.Services);
builder.Services.AddHealthChecks()
.AddCheck<StripeHealthCheck>("Stripe")
.AddSqlServer(_configuration["ConnectionStrings"])
.AddDbContextCheck<AppDbContext>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.QueueLimit = 0;
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromSeconds(20);
    });
});
builder.Services.AddControllers();
//.AddApplicationPart(assembly).AddApplicationPart(assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(setup =>
//{
//    // Include 'SecurityScheme' to use JWT Authentication
//    var jwtSecurityScheme = new OpenApiSecurityScheme
//    {
//        BearerFormat = "JWT",
//        Name = "JWT Authentication",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = JwtBearerDefaults.AuthenticationScheme,
//        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

//        Reference = new OpenApiReference
//        {
//            Id = JwtBearerDefaults.AuthenticationScheme,
//            Type = ReferenceType.SecurityScheme
//        }
//    };

//    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

//    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        { jwtSecurityScheme, Array.Empty<string>() }
//    });

//});
// Configure JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
//                .GetBytes(_configuration["AppSettings:Token"])),
//            ValidateIssuer = false,
//            ValidateAudience = false
//        };
//    });

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
}

app.UseHttpsRedirection();
app.MapHealthChecks("health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("fixed");
app.Run();
