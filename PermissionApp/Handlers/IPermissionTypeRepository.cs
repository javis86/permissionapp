using PermissionApp.Domain;

namespace PermissionApp.Handlers;

public interface IPermissionTypeRepository
{
    Task<PermissionType?> GetAsync(Guid permissionTypeId, CancellationToken cancellationToken);
}