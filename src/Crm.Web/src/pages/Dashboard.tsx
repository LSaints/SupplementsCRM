import { useState, useEffect, useCallback } from 'react';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Users, ShoppingCart, DollarSign, Package } from 'lucide-react';
import { dashboard } from '@/services/api/dashboard';

interface DashboardStats {
  totalClientes: number;
  totalPedidos: number;
  pedidosPagos: number;
  faturamentoTotal: number;
}

export function Dashboard() {
  const [stats, setStats] = useState<DashboardStats>({
    totalClientes: 0,
    totalPedidos: 0,
    pedidosPagos: 0,
    faturamentoTotal: 0,
  });
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    try {
      const data = await dashboard.getDados();
      setStats({
        totalClientes: data.totalClientes,
        totalPedidos: data.totalPedidos,
        pedidosPagos: data.pedidosPagos,
        faturamentoTotal: data.faturamentoTotal,
      });
    } catch {
      console.error('Erro ao carregar dados do dashboard');
    }
  }, []);

  useEffect(() => {
    setLoading(true);
    loadData().finally(() => setLoading(false));
    const interval = setInterval(loadData, 30000);
    return () => clearInterval(interval);
  }, [loadData]);

  const statCards = [
    { title: 'Clientes', value: stats.totalClientes, icon: Users },
    { title: 'Pedidos', value: stats.totalPedidos, icon: ShoppingCart },
    { title: 'Vendas Pagas', value: stats.pedidosPagos, icon: Package },
    { title: 'Receita', value: `R$ ${stats.faturamentoTotal.toFixed(2)}`, icon: DollarSign },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Dashboard</h1>
      {loading ? (
        <div className="text-muted-foreground">Carregando...</div>
      ) : (
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {statCards.map((stat) => {
          const Icon = stat.icon;
          return (
            <Card key={stat.title}>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">{stat.title}</CardTitle>
                <Icon className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">{stat.value}</div>
              </CardContent>
            </Card>
          );
        })}
      </div>
      )}
    </div>
  );
}