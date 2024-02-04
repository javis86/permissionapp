using MediatR;
using PermissionApp.Commands;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public RequestPermissionHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
        
        return true;
    }
}