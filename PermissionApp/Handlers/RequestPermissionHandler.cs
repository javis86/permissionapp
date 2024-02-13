using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
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
    private readonly ElasticsearchClient _elasticsearchClient;

    public RequestPermissionHandler(AppDbContext dbContext,
        ITopicProducer<KafkaMessage> kafkaProducer, 
        ElasticsearchClient elasticsearchClient)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
    }


    public record PermissionESTypeDto(int Id, Guid EmployeeID, Guid PermissionTypeId, Status Status)
    {
        public override string ToString()
        {
            return $"{{ Id = {Id}, EmployeeID = {EmployeeID}, PermissionTypeId = {PermissionTypeId}, Status = {Status} }}";
        }
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
       

        var permissionIndex = new PermissionESTypeDto(newRequestedPermission.Id,
            newRequestedPermission.Employee.Id,
            newRequestedPermission.PermissionType.Id,
            newRequestedPermission.Status);

        var response = await _elasticsearchClient.IndexAsync(permissionIndex, "permission-index");

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Index document with ID {response.Id} succeeded.");
        }

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