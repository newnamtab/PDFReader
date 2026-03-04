# Build the image
docker build -t pdfreader:latest .

# Run the container
docker run -p 5038:8080 pdfreader:latest

# Running the downloader
For now, it is a webproject
You need to call '/report'-endpoint on the server(and port) where its deployed:
E.g: localhost:5000/report

If it was a console project, you could just start it directly in program.cs