import { Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { DeleteDialog } from '@/components/ui/delete-dialog';
import { pedidos } from '@/services/api/pedidos';
import { checkout } from '@/services/api/checkout';
import type { Pedido } from '@/services/types';
import { Plus, Search, Pencil, Trash2, CreditCard } from 'lucide-react';

export function PedidosIndex() {
  const [items, setItems] = useState<Pedido[]>([]);
  const [loading, setLoading] = useState(true);
  const [busca, setBusca] = useState('');
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    loadItems();
  }, []);

  const loadItems = async () => {
    try {
      const data = await pedidos.getAll();
      setItems(data);
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredItems = items.filter((item) =>
    item.id.toLowerCase().includes(busca.toLowerCase())
  );

  const getStatusBadge = (status: string) => {
    const styles: Record<string, string> = {
      Pendente: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-100',
      Pago: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100',
      Cancelado: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-100',
    };
    return (
      <span className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${styles[status] || 'bg-gray-100 text-gray-800'}`}>
        {status}
      </span>
    );
  };

  const gerarLinkPagamento = async (pedidoId: string) => {
    try {
      const link = await checkout.criarLinkPagamento({ pedidoId });
      if (link.url) {
        window.open(link.url, '_blank');
      }
    } catch (error) {
      console.error('Erro ao gerar link:', error);
    }
  };

  const handleDelete = async () => {
    if (!selectedId) return;
    setDeleting(true);
    try {
      await pedidos.delete(selectedId);
      setDeleteOpen(false);
      setSelectedId(null);
      loadItems();
    } catch (error) {
      console.error('Erro ao excluir:', error);
    } finally {
      setDeleting(false);
    }
  };

  const openDelete = (id: string) => {
    setSelectedId(id);
    setDeleteOpen(true);
  };

  if (loading) return <div>Carregando...</div>;

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Pedidos</h1>
        <Link to="/pedidos/novo">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Novo
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Pedidos</CardTitle>
          <div className="flex items-center gap-2 pt-4">
            <div className="relative flex-1 max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar pedidos..."
                value={busca}
                onChange={(e) => setBusca(e.target.value)}
                className="pl-8"
              />
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Cliente</TableHead>
                <TableHead>Valor Total</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredItems.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.id.slice(0, 8)}...</TableCell>
                  <TableCell>{item.clienteNome || '-'}</TableCell>
                  <TableCell>R$ {item.valorTotal.toFixed(2)}</TableCell>
                  <TableCell>{getStatusBadge(item.status)}</TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      {item.status !== 'Pago' && (
                        <Button
                          variant="ghost"
                          size="icon-sm"
                          onClick={() => gerarLinkPagamento(item.id)}
                          title="Gerar link de pagamento"
                        >
                          <CreditCard className="h-4 w-4" />
                        </Button>
                      )}
                      {item.status !== 'Pago' && (
                        <Link to={`/pedidos/${item.id}`}>
                          <Button variant="ghost" size="icon-sm">
                            <Pencil className="h-4 w-4" />
                          </Button>
                        </Link>
                      )}
                      
                      <Button variant="ghost" size="icon-sm" onClick={() => openDelete(item.id)}>
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <DeleteDialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        onConfirm={handleDelete}
        title="Excluir Pedido"
        description="Tem certeza que deseja excluir este pedido? Esta ação não pode ser desfeita."
        loading={deleting}
      />
    </div>
  );
}