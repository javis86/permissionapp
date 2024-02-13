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
    private readonly ITopicProducer<KafkaMessage> _producerMs;

    private readonly IProducer<Null, string> _producerMS;
    // private readonly ITopicProducer<KafkaMessage> _producer;

    public RequestPermissionHandler(AppDbContext dbContext,
        //IProducer<Null, string> producer,
        ITopicProducer<KafkaMessage> producerMs)
    {
        _dbContext = dbContext;
        _producerMs = producerMs;
        //_producer = producer;
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


        // Confluent.Kafka
        // await _producer.ProduceAsync("permission-history",
        //     new Message<Null, string>
        //     {
        //         Value = Guid.NewGuid().ToString()
        //     });
        
        await _producerMs.Produce(new KafkaMessage() { Id = Guid.NewGuid() }, cancellationToken);

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