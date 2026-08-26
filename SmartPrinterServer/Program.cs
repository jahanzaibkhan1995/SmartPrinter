using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartPrinterServer.Data;
using SmartPrinterServer.Models;
using SmartPrinterServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// Database
builder.Services.AddDbContext<PrintDbContext>(options =>
    options.UseSqlite("Data Source=smartprint.db"));

// Configuration
builder.Services.Configure<PrintSettings>(
    builder.Configuration.GetSection("PrintSettings"));

builder.Services.Configure<PrinterOptions>(
    builder.Configuration.GetSection("Printer"));

// Services
builder.Services.AddScoped<IPrintJobService, PrintJobService>();

builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
builder.Services.AddSingleton<IPrintQueue, PrintQueue>();

builder.Services.AddSingleton<IPrinterService, WindowsPrinterService>();
builder.Services.AddSingleton<WindowsPrintTestService>();

// Background worker
builder.Services.AddHostedService<PrintWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
}

app.MapControllers();

app.Run();