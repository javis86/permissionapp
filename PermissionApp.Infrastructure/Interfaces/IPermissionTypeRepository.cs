using PermissionApp.Domain;

namespace PermissionApp.Infrastructure.Interfaces;

public interface IPermissionTypeRepository
{
    Task<PermissionType?> GetAsync(Guid permissionTypeId, CancellationToken cancellationToken);
}