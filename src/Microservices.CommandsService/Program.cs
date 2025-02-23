using Microservices.CommandsService.AsyncDataServices;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.EventProcessing;
using Microservices.CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
}

app.MapControllers();
PrepDb.PrepPopulation(app);

app.Run();
