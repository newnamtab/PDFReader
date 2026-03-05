using FileWriting;
using Microsoft.AspNetCore.Mvc;
using PdfUrlExaminer;
using Serilog;
using URLProvider;
using URLReader;



var builder = WebApplication.CreateBuilder(args);

// Add Serilog to the host
builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log.txt")
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog(dispose: true);
});

// Bind configuration to AppSettings
builder.Services.Configure<URLProviderSettings>(builder.Configuration.GetSection(nameof(URLProviderSettings)));
builder.Services.Configure<FileWriterSettings>(builder.Configuration.GetSection(nameof(FileWriterSettings)));
builder.Services.AddOptions();

// Add services to the container.
builder.Services.AddHttpClient("pdfClient", x => { x.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/pdf")); });

builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
builder.Services.AddScoped<IURLProvider, URLProvider.URLProvider>();
builder.Services.AddScoped<IURLReader, URLReader.URLReader>();
builder.Services.AddScoped<IBRNumberValidation, BRNumberValidation>();
builder.Services.AddScoped<IFileWriter, FileWriter>();
builder.Services.AddScoped<IPdfUrlExaminer, PdfUrlExaminer.PdfUrlExaminer>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/report", async ([FromServices] IURLProvider urlProvider,
                             [FromServices] IPdfUrlExaminer urlExaminer) =>
{
    var allUrls = urlProvider.GetURLs();

    await urlExaminer.ExamineUrls(allUrls.Select(u => (u.BRNumber, u.PrimaryUrl, u.SecondaryUrl)));
})
.WithName("GetReport");

app.Run();