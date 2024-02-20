using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class PermissionTypeRepository : IPermissionTypeRepository
{
    private readonly AppDbContext _dbContext;

    public PermissionTypeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PermissionType?> GetAsync(Guid permissionTypeId, CancellationToken cancellationToken)
    {
        return await _dbContext.PermissionTypes.FirstOrDefaultAsync(x => x.Id == permissionTypeId, cancellationToken);  
    }
}