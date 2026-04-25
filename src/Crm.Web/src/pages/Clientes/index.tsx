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
import { clientes } from '@/services/api/clientes';
import type { Cliente } from '@/services/types';
import { Plus, Search, Pencil, Trash2 } from 'lucide-react';

export function ClientesIndex() {
  const [items, setItems] = useState<Cliente[]>([]);
  const [loading, setLoading] = useState(true);
  const [busca, setBusca] = useState('');
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  const loadItems = async () => {
    try {
      const data = await clientes.getAll();
      setItems(data);
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadItems();
  }, []);

  const filteredItems = items.filter((item) =>
    (item.nome || '').toLowerCase().includes(busca.toLowerCase())
  );

  const handleDelete = async () => {
    if (!selectedId) return;
    setDeleting(true);
    try {
      await clientes.delete(selectedId);
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
        <h1 className="text-2xl font-bold">Clientes</h1>
        <Link to="/clientes/novo">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Novo
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Clientes</CardTitle>
          <div className="flex items-center gap-2 pt-4">
            <div className="relative flex-1 max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar clientes..."
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
                <TableHead>Nome</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Telefone</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredItems.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.nome || '-'}</TableCell>
                  <TableCell>{item.email || '-'}</TableCell>
                  <TableCell>{item.telefone || '-'}</TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Link to={`/clientes/${item.id}`}>
                        <Button variant="ghost" size="icon-sm">
                          <Pencil className="h-4 w-4" />
                        </Button>
                      </Link>
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
        title="Excluir Cliente"
        description="Tem certeza que deseja excluir este cliente? Esta ação não pode ser desfeita."
        loading={deleting}
      />
    </div>
  );
}