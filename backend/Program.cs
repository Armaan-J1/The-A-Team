using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;


var builder = WebApplication.CreateBuilder(args);

// Register EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=allocation.db")
);

// Register your services
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<CoordinatorService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<CoordinatorService>();
builder.Services.AddScoped<SessionService>();

// CORS so the Vite frontend can call us
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create DB and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    backend.Data.SeedData.Run(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();