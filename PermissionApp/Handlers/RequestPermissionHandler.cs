using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var permission = await _dbContext.Permissions.Include(x => x.Employee).Include(x => x.PermissionType)
            .FirstOrDefaultAsync(x =>
                x.Employee.Id == request.EmployeeId && x.PermissionType.Id == request.PermissionTypeId);

        if (permission is null)
        {
            var permissionType = _dbContext.PermissionTypes.FirstOrDefault(x => x.Id == request.PermissionTypeId);

            if (permissionType is null)
                throw new KeyNotFoundException(nameof(PermissionType));

            var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == request.EmployeeId);
            if (employee is null)
                throw new KeyNotFoundException(nameof(Employee));

            permission = permissionType.RequestFor(employee);

            await _dbContext.Permissions.AddAsync(permission, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (permission.Status != Status.Requested)
                return false;
        }

        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "request"
            }, cancellationToken);


        await _elasticsearchClient.IndexAsync(new PermissionESTypeDto(permission.Id,
            permission.Employee.Id,
            permission.PermissionType.Id,
            permission.Status), "permission-index");

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