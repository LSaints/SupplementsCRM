namespace Crm.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalClientes { get; set; }
    public int TotalPedidos { get; set; }
    public int PedidosPagos { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public Dictionary<string, int> PedidosStatus { get; set; } = new();
}