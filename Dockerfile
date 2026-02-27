# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file
COPY ["PDFReader.sln", "."]

# Copy project files
COPY ["PDFReader/PDFReader.csproj", "PDFReader/"]
COPY ["PDFUrlExaminer/PDFUrlExaminer.csproj", "PDFUrlExaminer/"]
COPY ["URLProvider/URLProvider.csproj", "URLProvider/"]
COPY ["URLReader/URLReader.csproj", "URLReader/"]
COPY ["PDFUrlExaminer.Tests/PDFUrlExaminer.Tests.csproj", "PDFUrlExaminer.Tests/"]
COPY ["URLReader.Tests/URLReader.Tests.csproj", "URLReader.Tests/"]
COPY ["URLProvider.Tests/URLProvider.Tests.csproj", "URLProvider.Tests/"]

# Restore dependencies
RUN dotnet restore "PDFReader.sln"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "PDFReader/PDFReader.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PDFReader/PDFReader.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create directories needed at runtime
RUN mkdir -p FileUpload FileStorage

# Expose port (default ASP.NET Core port)
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "PDFReader.dll"]
