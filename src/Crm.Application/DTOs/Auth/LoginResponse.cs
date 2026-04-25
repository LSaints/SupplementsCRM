namespace Crm.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}