using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Handlers;
using PermissionApp.Infrastructure.Interfaces;

namespace PermissionApp.Infrastructure.Repositories;

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