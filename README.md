# MyBank
ASP.NET Core portfolio project.

## How to run MyBank:

### Option 1 (In Visual Studio):
1. Copy `.env.example` to `.env` and edit it if needed.
2. Open `MyBank.slnx` in Visual Studio.
3. In solution explorer right click on docker-compose and then click "Set As Startup Project".
4. Choose `Container (Dockerfile)` configuration and run the project.

### Option 2 (Outside of IDE):
1. Prepare SSL certificate. You can use `dotnet dev-certs https` to create certificate for local use. 
More info about `dotnet dev-certs https` can be found [here](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https).
Note: use appropriate slashes for the host OS.
2. Copy `.env.example` to `.env` and set `HOST_CERT_PATH` and `CERT_PASSWORD` and other variables in `.env` to match your requirements and certificate.
Use absolute paths.
3. Run `docker compose -f "docker-compose.dev.yml" up`.
4. Open `https://localhost` in your browser.



