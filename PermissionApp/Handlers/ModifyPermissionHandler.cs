using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly IPermissionRepository _permissionRepository;
    private IUnitOfWork _unitOfWork;

    public ModifyPermissionHandler(AppDbContext dbContext,
        ITopicProducer<KafkaMessage> kafkaProducer, 
        ElasticsearchClient elasticsearchClient,
        IPermissionRepository permissionRepository, 
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetAsync(request.EmployeeId,
            request.PermissionTypeId,
            cancellationToken);

        if (permission is null)
            throw new KeyNotFoundException(nameof(Permission));
        
        switch (request.PermissionCommandType)
        {
            case PermissionCommandType.Grant: permission.Grant();
                break;
            case PermissionCommandType.Deny: permission.Deny();
                break;
        }

        _permissionRepository.Update(permission, cancellationToken);
        _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "modify"
            }, cancellationToken);
        
        await _elasticsearchClient.IndexAsync(new PermissionESTypeDto(permission.Id,
            permission.Employee.Id,
            permission.PermissionType.Id,
            permission.Status), "permission-index");
        
        return true;
    }
}