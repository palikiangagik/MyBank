# MyBank
ASP.NET Core portfolio project.

---

## Prerequisites
Before you begin, ensure you have the following installed:
* **.NET 8.0 SDK** or later.
* **Docker Desktop** (or Docker Engine with Compose).
* **Visual Studio 2022/2026** (for IDE-based development).

---

## How to run MyBank

### Option 1 (In Visual Studio):
1. Copy `.env.example` to `.env` in the root directory. 
2. Open `MyBank.slnx` in Visual Studio.
3. In Solution Explorer, click on docker-compose and then click "Set As Startup Project".
4. Click `Docker Compose` to run the project.

### Option 2 (Outside of IDE):
1. Prepare SSL certificate. You can use `dotnet dev-certs https` to create certificate for local use. 
More info about `dotnet dev-certs https` can be found [here](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https).
Note: use appropriate slashes for the host OS.
2. Copy `.env.example` to `.env` and set `HOST_CERT_PATH` and `CERT_PASSWORD` and other variables in `.env` to match your requirements and certificate.
Use absolute paths.
3. Run `docker compose -f "docker-compose.dev.yml" up`.
4. Open `localhost` in your browser.



