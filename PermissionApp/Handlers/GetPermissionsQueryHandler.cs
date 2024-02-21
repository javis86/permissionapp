using AutoMapper;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;
using PermissionApp.Infrastructure.Interfaces;

namespace PermissionApp.Handlers;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Employee?>
{
    private readonly AppDbContext _dbContext;
    private readonly ITopicProducer<KafkaMessage> _kafkaProducer;
    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly IEmployeeRepository _employeeRepository;

    public GetPermissionsQueryHandler(AppDbContext dbContext, IMapper mapper,
        ITopicProducer<KafkaMessage> kafkaProducer, 
        ElasticsearchClient elasticsearchClient, 
        IEmployeeRepository employeeRepository)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
        _employeeRepository = employeeRepository;
    }
    
    public async Task<Employee?> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "get"
            }, cancellationToken);

        var employee = await _employeeRepository.GetAsync(request.EmployeeId, cancellationToken);

        foreach (var permission in employee?.Permissions)
        {
            await _elasticsearchClient.IndexAsync(new PermissionESTypeDto(permission.Id,
                permission.Employee.Id,
                permission.PermissionType.Id,
                permission.Status), "permission-index");
        }
        
        return employee;
    }
}