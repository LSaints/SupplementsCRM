using System.Security.Claims;

namespace Crm.Domain.Extensions;

public static class ClaimExtensions
{
    public static Guid GetUsuarioId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.GetRole() == "Admin";
    }
}