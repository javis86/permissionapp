using MediatR;
using Microsoft.EntityFrameworkCore;
using PermissionApp.Commands;
using PermissionApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    // DbInitializer.Initialize(context);
}

app.MapPost("/RequestPermission", async (IMediator mediator, RequestPermissionCommand command) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapPost("/ModifyPermission", async (IMediator mediator, ModifyPermissionCommand command) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapGet("/GetPermissions", async (IMediator mediator) =>
{
    var query = new GetPermissionsQuery();
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

app.Run();