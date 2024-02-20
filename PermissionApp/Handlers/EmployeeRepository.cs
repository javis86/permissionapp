using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Employee?> GetAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees.Include(x => x.Permissions)
            .ThenInclude(x => x.PermissionType)
            .Where(x => x.Id == employeeId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}