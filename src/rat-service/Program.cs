using rat_service_core.Interfaces;
using rat_service_core.Entities;
using rat_service_infrastructure.Services;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CloudStorageOptions>(builder.Configuration.GetSection("Cloud:Storage"));

// Google Storage 
builder.Services.AddScoped<StorageClientFactory>();
builder.Services.AddScoped<ICloudStorageClient, GoogleCloudStorageClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program {}
