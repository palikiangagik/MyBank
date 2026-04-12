# MyBank
ASP.NET Core portfolio project.

## How to run:

### Option 1:
1. Copy `.env.example` to `.env`.
2. Open `MyBank.slnx` in Visual Studio
3. Choose `Container (Dockerfile)` configuration and run the project.

### Option 2 (using Docker):
1. Prepare SSL certificate. You can use `dotnet dev-certs https` to create certificate for local use. 
More info about `dotnet dev-certs https` can be found [here](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https).
Note: use appropriate slashes for the host OS.
2. Copy `.env.example` to `.env` and set `HOST_CERT_PATH` and `CERT_PASSWORD` and other variables in `.env` to match your requirements and certificate.
Use absolute paths.
3. Run `docker compose -f "docker-compose.yml" up`.
4. Open `https://localhost` in your browser.



