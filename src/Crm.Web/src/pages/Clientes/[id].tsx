import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { DeleteDialog } from '@/components/ui/delete-dialog';
import { clientes } from '@/services/api/clientes';

export function EditarCliente() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [carregando, setCarregando] = useState(true);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [telefone, setTelefone] = useState('');

  const loadCliente = useCallback(async (clienteId: string) => {
    setCarregando(true);
    try {
      const data = await clientes.getById(clienteId);
      setNome(data.nome || '');
      setEmail(data.email || '');
      setTelefone(data.telefone || '');
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setCarregando(false);
    }
  }, []);

  useEffect(() => {
    if (id) {
      loadCliente(id);
    }
  }, [id, loadCliente]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;
    setLoading(true);
    try {
      await clientes.update(id, {
        nome,
        email,
        telefone,
      });
      navigate('/clientes');
    } catch (error) {
      console.error('Erro ao atualizar:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!id) return;
    setLoading(true);
    try {
      await clientes.delete(id);
      navigate('/clientes');
    } catch (error) {
      console.error('Erro ao deletar:', error);
    } finally {
      setLoading(false);
    }
  };

  if (carregando) return <div>Carregando...</div>;

  return (
    <div className="max-w-xl">
      <Card>
        <CardHeader>
          <CardTitle>Editar Cliente</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="nome">Nome</Label>
              <Input
                id="nome"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="telefone">Telefone</Label>
              <Input
                id="telefone"
                type="tel"
                value={telefone}
                onChange={(e) => setTelefone(e.target.value)}
              />
            </div>
            <div className="flex gap-2">
              <Button type="submit" disabled={loading}>
                {loading ? 'Salvando...' : 'Salvar'}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate('/clientes')}>
                Cancelar
              </Button>
              <Button type="button" variant="destructive" onClick={() => setDeleteOpen(true)} disabled={loading}>
                Excluir
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>

      <DeleteDialog
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        onConfirm={handleDelete}
        title="Excluir Cliente"
        description="Tem certeza que deseja excluir este cliente? Esta ação não pode ser desfeita."
        loading={loading}
      />
    </div>
  );
}