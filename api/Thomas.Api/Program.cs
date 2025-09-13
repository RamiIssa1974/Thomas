using Microsoft.EntityFrameworkCore;
using Thomas.Api.Application.Interfaces;
using Thomas.Api.Application.Services;
using Thomas.Api.Infrastructure;
using Thomas.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
const string CorsDev = "CorsDev";
builder.Services.AddCors(o =>
{
    o.AddPolicy(CorsDev, p =>
        p.WithOrigins("http://localhost:4200", "https://localhost:4200") // Angular dev
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials() // נשאיר ON כדי שבעתיד יהיה אפשר קב׳/JWT עם cookies
    );
});
// Minimal services; keep Swagger ready from day one
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ThomasDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ThomasDb")));

builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IExamBankRepository, ExamBankRepository>();
builder.Services.AddScoped<IExamBankService, ExamBankService>();
builder.Services.AddScoped<IAttemptRepository, AttemptRepository>();
builder.Services.AddScoped<IAttemptService, AttemptService>();


builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsDev);
app.MapControllers(); 

app.Run();
