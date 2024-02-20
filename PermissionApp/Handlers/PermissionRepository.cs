using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _dbContext;

    public PermissionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken)
    {
        await _dbContext.Permissions.AddAsync(permission, cancellationToken);
    }

    public void Update(Permission permission, CancellationToken cancellationToken)
    {
        _dbContext.Permissions.Update(permission);
    }

    public async Task<Permission?> GetAsync(Guid employeeId, Guid permissionTypeId, CancellationToken cancellationToken)
    {
        return await _dbContext.Permissions.Include(x => x.Employee)
            .Include(x => x.PermissionType)
            .FirstOrDefaultAsync(x =>
                x.Employee.Id == employeeId && x.PermissionType.Id == permissionTypeId, 
                cancellationToken);
    }
}