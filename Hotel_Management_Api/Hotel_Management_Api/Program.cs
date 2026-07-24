// 1. IMPORT NAMESPACES
using Hotel_Management_Api.Interfaces;
using Hotel_Management_Api.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- PHASE 1: CONFIGURE SERVICES ---
// Add services to the container before building the app.

builder.Services.AddControllers();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection: Registering our raw ADO.NET repository
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// --- BUILD THE APPLICATION ---
// This line separates the Service configuration from the Middleware pipeline.
var app = builder.Build();

// --- PHASE 2: CONFIGURE THE HTTP REQUEST PIPELINE (MIDDLEWARE) ---

// Enable Swagger UI for API testing (usually only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Start the server
app.Run();