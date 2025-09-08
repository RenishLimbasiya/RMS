using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMS.Data;
using RMS.Models.Entities;
using RMS.Services;
using RMS.Hubs;
using RMS.Repositories.Interfaces;
using RMS.Repositories.Implementations;
using System.Text;
using Serilog;
using FluentValidation;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Serilog
// ----------------------------
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

// ----------------------------
// Database
// ----------------------------
builder.Services.AddDbContext<RmsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con")));

// ----------------------------
// Identity
// ----------------------------
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<RmsDbContext>()
    .AddDefaultTokenProviders();

// ----------------------------
// JWT Authentication
// ----------------------------
var jwt = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };

    // ✅ Allow SignalR to receive access token via query string
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/kdshub"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

// ----------------------------
// CORS
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ----------------------------
// Controllers + JSON
// ----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ----------------------------
// Swagger + SignalR
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// ----------------------------
// DI – Services & Repositories
// ----------------------------
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<TableService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<BillingService>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();

var app = builder.Build();

// ----------------------------
// Seed Data
// ----------------------------
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// ----------------------------
// Middleware
// ----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");       // ✅ CORS first
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<KdsHub>("/kdshub");    // ✅ SignalR hub

app.Run();
