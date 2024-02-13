using Confluent.Kafka;
using MassTransit;
using MediatR;
using PermissionApp.Commands;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, bool>
{
    private readonly AppDbContext _dbContext;
    private readonly ITopicProducer<KafkaMessage> _kafkaProducer;

    public RequestPermissionHandler(AppDbContext dbContext,
        ITopicProducer<KafkaMessage> kafkaProducer)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
    }


    public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        var permissionType = _dbContext.PermissionTypes.FirstOrDefault(x => x.Id == request.PermissionTypeId);

        if (permissionType is null)
            throw new KeyNotFoundException(nameof(PermissionType));

        var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
        if (employee is null)
            throw new KeyNotFoundException(nameof(Employee));

        var newRequestedPermission = permissionType.RequestFor(employee);

        await _dbContext.Permissions.AddAsync(newRequestedPermission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "request"
            }, cancellationToken);

        return true;
    }
}

public static class KafkaProducerConfig
{
    public static ProducerConfig GetConfig()
    {
        return new ProducerConfig
        {
            BootstrapServers = "kafka:9092", // Replace with your Kafka broker address
            ClientId = "KafkaExampleProducer",
        };
    }
}