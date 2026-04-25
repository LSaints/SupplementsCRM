import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { produtos } from '@/services/api/produtos';

export function EditarProduto() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [carregando, setCarregando] = useState(true);
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [preco, setPreco] = useState('');
  const [ativo, setAtivo] = useState(true);

  const loadProduto = useCallback(async (produtoId: string) => {
    try {
      const data = await produtos.getById(produtoId);
      setNome(data.nome);
      setDescricao(data.descricao || '');
      setPreco(data.preco.toString());
      setAtivo(data.ativo);
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setCarregando(false);
    }
  }, []);

  useEffect(() => {
    if (id) {
      loadProduto(id).then(() => {});
    }
  }, [id, loadProduto]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id) return;
    setLoading(true);
    try {
      await produtos.update(id, {
        nome,
        descricao,
        preco: parseFloat(preco),
        ativo,
      });
      navigate('/produtos');
    } catch (error) {
      console.error('Erro ao atualizar:', error);
    } finally {
      setLoading(false);
    }
  };

  if (carregando) return <div>Carregando...</div>;

  return (
    <div className="max-w-xl">
      <Card>
        <CardHeader>
          <CardTitle>Editar Produto</CardTitle>
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
              <Label htmlFor="descricao">Descrição</Label>
              <Textarea
                id="descricao"
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="preco">Preço</Label>
              <Input
                id="preco"
                type="number"
                step="0.01"
                min="0"
                value={preco}
                onChange={(e) => setPreco(e.target.value)}
                required
              />
            </div>
            <div className="flex items-center justify-between">
              <Label htmlFor="ativo">Ativo</Label>
              <Switch
                id="ativo"
                checked={ativo}
                onCheckedChange={setAtivo}
              />
            </div>
            <div className="flex gap-2">
              <Button type="submit" disabled={loading}>
                {loading ? 'Salvando...' : 'Salvar'}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate('/produtos')}>
                Cancelar
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}