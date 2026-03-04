using FileWriting;
using Microsoft.AspNetCore.Mvc;
using PdfUrlExaminer;
using URLProvider;
using URLReader;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("pdfClient", x => { x.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/pdf")); });

builder.Services.AddScoped<IURLProvider, URLProvider.URLProvider>(sp =>
{
    var filePath = builder.Configuration.GetValue<string>("PDFReader:FilePath") ?? "";
    var brNumberColumnName = builder.Configuration.GetValue<string>("PDFReader:BRNumberColumnName") ?? "";
    var primaryColumnName = builder.Configuration.GetValue<string>("PDFReader:PrimaryColumnName") ?? "";
    var secondaryColumnName = builder.Configuration.GetValue<string>("PDFReader:SecondaryColumnName") ?? "";

    return new URLProvider.URLProvider(filePath, brNumberColumnName, primaryColumnName, secondaryColumnName);
});
builder.Services.AddScoped<IURLReader, URLReader.URLReader>();
builder.Services.AddScoped<IBRNumberValidation, BRNumberValidation>(sp =>
{
    var brNumberRegex = builder.Configuration.GetValue<string>("PDFReader:BRNumberRegex") ?? "";
    return new BRNumberValidation(brNumberRegex);
});
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