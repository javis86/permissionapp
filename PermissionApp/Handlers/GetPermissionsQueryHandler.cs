using AutoMapper;
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

    public GetPermissionsQueryHandler(AppDbContext dbContext, IMapper mapper,
        ITopicProducer<KafkaMessage> kafkaProducer)
    {
        _dbContext = dbContext;
        _kafkaProducer = kafkaProducer;
    }
    
    public async Task<Employee?> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        await _kafkaProducer.Produce(
            new KafkaMessage()
            {
                Id = Guid.NewGuid(),
                NameOperation = "get"
            }, cancellationToken);
        
        return await _dbContext.Employees.Include(x => x.Permissions)
            .ThenInclude(x => x.PermissionType)
            .Where(x => x.Id == request.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}