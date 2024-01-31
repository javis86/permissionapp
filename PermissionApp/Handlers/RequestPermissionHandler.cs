using MediatR;
using PermissionApp.Commands;

namespace PermissionApp.Handlers;

public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, bool>
{
    public Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
    {
        // Lógica para procesar la solicitud de permiso
        // Devuelve true si la solicitud se procesa con éxito, de lo contrario, false.
        return Task.FromResult(true);
    }
}