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
import { produtos } from '@/services/api/produtos';
import type { Produto } from '@/services/types';
import { Plus, Search, Pencil, Trash2 } from 'lucide-react';

export function ProdutosIndex() {
  const [items, setItems] = useState<Produto[]>([]);
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
      const data = await produtos.getAll(true);
      setItems(data);
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredItems = items.filter((item) =>
    item.nome.toLowerCase().includes(busca.toLowerCase())
  );

  const handleDelete = async () => {
    if (!selectedId) return;
    setDeleting(true);
    try {
      await produtos.delete(selectedId);
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
        <h1 className="text-2xl font-bold">Produtos</h1>
        <Link to="/produtos/novo">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Novo
          </Button>
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Lista de Produtos</CardTitle>
          <div className="flex items-center gap-2 pt-4">
            <div className="relative flex-1 max-w-sm">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar produtos..."
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
                <TableHead>Preço</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Ações</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredItems.map((item) => (
                <TableRow key={item.id}>
                  <TableCell className="font-medium">{item.nome}</TableCell>
                  <TableCell>R$ {item.preco.toFixed(2)}</TableCell>
                  <TableCell>
                    <span
                      className={`inline-flex items-center rounded-full px-2 py-1 text-xs ${
                        item.ativo
                          ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100'
                          : 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200'
                      }`}
                    >
                      {item.ativo ? 'Ativo' : 'Inativo'}
                    </span>
                  </TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Link to={`/produtos/${item.id}`}>
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
        title="Excluir Produto"
        description="Tem certeza que deseja excluir este produto? Esta ação não pode ser desfeita."
        loading={deleting}
      />
    </div>
  );
}