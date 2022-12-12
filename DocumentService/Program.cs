using Microsoft.EntityFrameworkCore;
using DocumentService.Data;
using System.IO.Abstractions;
using DocumentService.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DocumentMetadataContext") ?? throw new InvalidOperationException("Connection string 'DocumentMetadataContext' not found.")));

builder.Services.AddScoped<IFileStorageManager, HddManager>();
builder.Services.AddScoped<IMetadataManager, MetadataManager>();
builder.Services.AddScoped<IFileSystem, FileSystem>();

builder.Services.AddControllers().AddXmlSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
