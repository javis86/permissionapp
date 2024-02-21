using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Handlers;
using PermissionApp.Infrastructure.Interfaces;

namespace PermissionApp.Infrastructure.Repositories;

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