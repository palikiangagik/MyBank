using MyBank.Application;
using MyBank.Infrastructure;
using MyBank.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("MyBankDBConnection")
    ?? throw new InvalidOperationException("Connection string 'MyBankDBConnection' is missing. Check the config.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMyBankApplication();
builder.Services.AddMyBankInfrastructure(connectionString);
builder.Services.AddMyBankIdentity();

var app = builder.Build();

await app.InitializeMyBankDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
