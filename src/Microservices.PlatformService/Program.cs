using Microservices.PlatformService.AsyncDataServices;
using Microservices.PlatformService.Data;
using Microservices.PlatformService.SyncDataServices;
using Microservices.PlatformService.SyncDataServices.Http;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

var isProduction = builder.Environment.IsProduction();
//if (isProduction)
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(
            "Server=mssql-clusterip-srv,1433;Initial Catalog=platformsdbtest;" +
            "User ID=sa;Password=stas110802;TrustServerCertificate=True;Persist Security Info=True;Encrypt=True"));
}

// if (builder.Environment.IsDevelopment())
// {
//     builder.Services.AddDbContext<AppDbContext>(opt =>
//         opt.UseInMemoryDatabase("InMemoryDb"));
// }

var app = builder.Build();
PrepDb.PrepPopulation(app, isProduction);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
}

var msgBusClient = app.Services.GetService<IMessageBusClient>();
if(msgBusClient != null)
    await msgBusClient.CreateConnectAsync();

app.MapControllers();
app.Run();