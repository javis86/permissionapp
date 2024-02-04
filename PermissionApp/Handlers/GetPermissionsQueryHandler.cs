using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PermissionApp.Domain;
using PermissionApp.Infrastructure;

namespace PermissionApp.Handlers;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Employee?>
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetPermissionsQueryHandler(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    
    public async Task<Employee?> Handle(GetPermissionsQuery request, CancellationToken cancellationToken) =>
        await _dbContext.Employees.Include(x => x.Permissions)
            .ThenInclude(x => x.PermissionType)
            .Where(x => x.Id == request.EmployeeId)
            .FirstOrDefaultAsync();
}