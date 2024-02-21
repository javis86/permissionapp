using PermissionApp.Domain;

namespace PermissionApp.Infrastructure.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetAsync(Guid employeeId, CancellationToken cancellationToken);
}