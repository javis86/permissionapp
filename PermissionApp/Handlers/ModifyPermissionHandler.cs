using MediatR;
using PermissionApp.Commands;

namespace PermissionApp.Handlers;

public class ModifyPermissionHandler : IRequestHandler<ModifyPermissionCommand, bool>
{
    public Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
    {
        // Lógica para modificar los permisos
        // Devuelve true si la modificación se realiza con éxito, de lo contrario, false.
        return Task.FromResult(true);
    }
}