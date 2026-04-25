namespace Crm.Domain;

public static class Errors
{
    public static Error NotFound { get; } = new("NotFound", "Recurso não encontrado.");
    public static Error Unauthorized { get; } = new("Unauthorized", "Não autenticado.");
    public static Error Forbidden { get; } = new("Forbidden", "Acesso negado.");
    public static Error Validation { get; } = new("Validation", "Dados inválidos.");

    public static Error UsuarioNaoEncontrado(Guid id) =>
        new("UsuarioNaoEncontrado", $"Usuário {id} não encontrado.");

    public static Error LeadNaoEncontrado(Guid id) =>
        new("LeadNaoEncontrado", $"Lead {id} não encontrado.");

    public static Error ClienteNaoEncontrado(Guid id) =>
        new("ClienteNaoEncontrado", $"Cliente {id} não encontrado.");

    public static Error ProdutoNaoEncontrado(Guid id) =>
        new("ProdutoNaoEncontrado", $"Produto {id} não encontrado.");

    public static Error PedidoNaoEncontrado(Guid id) =>
        new("PedidoNaoEncontrado", $"Pedido {id} não encontrado.");
}

public class Error
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }
}