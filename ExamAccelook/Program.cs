using ExamAccelook.Entities;
using ExamAccelook.Logics.Handlers;
using ExamAccelook.Logics.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

// Ensure the Log directory exists
Directory.CreateDirectory("logs");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.UseSerilog();

// Configure SQL Server
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<ExamAccelookContext>(options =>
{
    var conString = configuration.GetConnectionString("SQLServerDB");
    options.UseSqlServer(conString);
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAvailableTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<PostBookTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetBookedTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<EditBookedTicketHandler>());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RevokeTicketHandler>());

builder.Services.AddValidatorsFromAssemblyContaining<GetAvailableTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PostBookTicketValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetBookedTicketHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<EditBookedTicketHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<RevokeTicketValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();