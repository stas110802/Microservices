using Microservices.PlatformService.AsyncDataServices;
using Microservices.PlatformService.Data;
using Microservices.PlatformService.SyncDataServices;
using Microservices.PlatformService.SyncDataServices.Grpc;
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
builder.Services.AddGrpc();

var isProduction = builder.Environment.IsProduction();
if (isProduction)
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConnection")));
}

var app = builder.Build();

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
app.MapGrpcService<GrpcPlatformService>();

// not necessary
app.MapGet("/protos/platform.proto", async context => 
    await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto")));
PrepDb.PrepPopulation(app, isProduction);

app.Run();