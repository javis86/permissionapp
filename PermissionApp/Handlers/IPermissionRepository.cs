using PermissionApp.Domain;

namespace PermissionApp.Handlers;

public interface IPermissionRepository
{
    Task AddAsync(Permission permission, CancellationToken cancellationToken);
    void Update(Permission permission, CancellationToken cancellationToken);
    Task<Permission?> GetAsync(Guid employeeId, Guid permissionTypeId, CancellationToken cancellationToken);
}