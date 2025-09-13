using Microsoft.EntityFrameworkCore;
using Thomas.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Minimal services; keep Swagger ready from day one
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ThomasDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ThomasDb")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();
