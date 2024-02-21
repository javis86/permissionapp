using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PermissionApp.Commands;
using PermissionApp.Contracts;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;
using PermissionApp.Infrastructure.Interfaces;

namespace PermissionApp.Handlers;

public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, bool>
{
    private readonly ITopicProducer<KafkaMessage> _kafkaProducer;
    private readonly ElasticsearchClient _elasticsearchClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPermissionTypeRepository _permissionTypeRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public RequestPermissionHandler(ITopicProducer<KafkaMessage> kafkaProducer,
        ElasticsearchClient elasticsearchClient, 
        IUnitOfWork unitOfWork,
        IPermissionRepository permissionRepository,
        IPermissionTypeRepository permissionTypeRepository,
        IEmployeeRepository employeeRepository)
    {
        _kafkaProducer = kafkaProducer;
        _elasticsearchClient = elasticsearchClient;
        _unitOfWork = unitOfWork;
        _permissionRepository = permissionRepository;
        _permissionTypeRepository = permissionTypeRepository;
        _employeeRepository = employeeRepository;
    }


    public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await _permissionRepository.GetAsync(request.EmployeeId, request.PermissionTypeId, cancellationToken);

        if (permission is null)
        {
            var permissionType = await _permissionTypeRepository.GetAsync(request.PermissionTypeId, cancellationToken);
            if (permissionType is null)
                throw new KeyNotFoundException(nameof(PermissionType));

            var employee = await _employeeRepository.GetAsync(request.EmployeeId, cancellationToken);
            if (employee is null)
                throw new KeyNotFoundException(nameof(Employee));

            permission = permissionType.RequestFor(employee);

            await _permissionRepository.AddAsync(permission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
            permission.Status), "permission-index", cancellationToken);

        return true;
    }
}
