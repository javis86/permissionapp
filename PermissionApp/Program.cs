using AutoMapper;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PermissionApp;
using PermissionApp.Commands;
using PermissionApp.Contracts;
using PermissionApp.Infrastructure;
using PermissionApp.Models;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory();

    x.AddRider(rider =>
    {
        rider.AddProducer<KafkaMessage>("permission-history");
        
        rider.UsingKafka((context, k) => { k.Host( builder.Configuration.GetSection("KafkaConfig").Get<string>()); });
    });
});

builder.Services.AddSingleton<ElasticsearchClient>(provider =>
    new ElasticsearchClient(new Uri(builder.Configuration.GetSection("ElasticsearchConfig").Get<string>())));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configuration) => configuration.MinimumLevel.Information().WriteTo.Console()); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.MapPost("/RequestPermission", async (IMediator mediator, ILogger logger, RequestPermissionCommand command) =>
{
    logger.Information("Operation Request");
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapPost("/ModifyPermission", async (IMediator mediator, ILogger logger, ModifyPermissionCommand command) =>
{
    logger.Information("Operation Modify");
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapPost("/GetPermissions", async (IMediator mediator, ILogger logger, IMapper mapper, GetPermissionsQuery query) =>
{
    logger.Information("Operation Get");
    var result = mapper.Map<PermissionsDto>(await mediator.Send(query));
    return Results.Ok(result);
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();