using MediatR;
using PermissionApp.Models;

namespace PermissionApp.Handlers;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
{
    public Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        // Lógica para obtener la lista de permisos
        // Devuelve la lista de permisos
        var permissions = new List<PermissionDto>
        {
            new PermissionDto { /* Propiedades del primer permiso */ },
            new PermissionDto { /* Propiedades del segundo permiso */ },
            // Agrega más permisos según sea necesario
        };

        return Task.FromResult(permissions);
    }
}