using PermissionApp.Domain;

namespace PermissionApp.Handlers;

public interface IEmployeeRepository
{
    Task<Employee?> GetAsync(Guid employeeId, CancellationToken cancellationToken);
}