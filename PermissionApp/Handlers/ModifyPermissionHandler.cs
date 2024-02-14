using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using PermissionApp.Commands;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class ModifyPermissionHandler : IRequestHandler<ModifyPermissionCommand, bool>
{
    private readonly AppDbContext _dbContext;
    private readonly ITopicProducer<KafkaMessage> _kafkaProducer;
    private readonly ElasticsearchClient _elasticsearchClient;

    public ModifyPermissionHandler(AppDbContext dbContext,
        ITopicProducer<KafkaMessage> kafkaProducer, 
        ElasticsearchClient elasticsearchClient)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
    }
    
    public async Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = _dbContext.Permissions.FirstOrDefault(x => x.PermissionType.Id == request.PermissionTypeId && x.Employee.Id == request.EmployeeId);

        if (permission is null)
            throw new KeyNotFoundException(nameof(Permission));
        
        switch (request.PermissionCommandType)
        {
            case PermissionCommandType.Grant: permission.Grant();
                break;
            case PermissionCommandType.Deny: permission.Deny();
                break;
        }

        _dbContext.Permissions.Update(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "modify"
            }, cancellationToken);
        
        await _elasticsearchClient.IndexAsync(new RequestPermissionHandler.PermissionESTypeDto(permission.Id,
            permission.Employee.Id,
            permission.PermissionType.Id,
            permission.Status), "permission-index");
        
        return true;
    }
}