using AutoMapper;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Employee?>
{
    private readonly AppDbContext _dbContext;
    private readonly ITopicProducer<KafkaMessage> _kafkaProducer;
    private readonly ElasticsearchClient _elasticsearchClient;

    public GetPermissionsQueryHandler(AppDbContext dbContext, IMapper mapper,
        ITopicProducer<KafkaMessage> kafkaProducer, 
        ElasticsearchClient elasticsearchClient)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
    }
    
    public async Task<Employee?> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "get"
            }, cancellationToken);
        
        var employee = await _dbContext.Employees.Include(x => x.Permissions)
            .ThenInclude(x => x.PermissionType)
            .Where(x => x.Id == request.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);

        foreach (var permission in employee.Permissions)
        {
            await _elasticsearchClient.IndexAsync(new RequestPermissionHandler.PermissionESTypeDto(permission.Id,
                permission.Employee.Id,
                permission.PermissionType.Id,
                permission.Status), "permission-index");
        }
        
        return employee;
    }
}