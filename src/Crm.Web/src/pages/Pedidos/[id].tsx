import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { pedidos, type CreatePedidoRequest } from '@/services/api/pedidos';
import { clientes as clientesApi } from '@/services/api/clientes';
import { produtos as produtosApi } from '@/services/api/produtos';
import type { Cliente, Produto } from '@/services/types';
import { Trash2, Plus } from 'lucide-react';

export function EditarPedido() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [carregando, setCarregando] = useState(true);
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [clienteId, setClienteId] = useState('');
  const [status, setStatus] = useState('');
  const [itens, setItens] = useState<{ produtoId: string; quantidade: number }[]>([]);

  const loadData = useCallback(async () => {
    try {
      const [clientesData, produtosData, pedidoData] = await Promise.all([
        clientesApi.getAll(),
        produtosApi.getAll(true),
        id ? pedidos.getById(id) : Promise.resolve(null),
      ]);
      setClientes(clientesData);
      setProdutos(produtosData);
      if (pedidoData) {
        setClienteId(pedidoData.clienteId);
        setStatus(pedidoData.status);
        setItens(
          pedidoData.itens.map((item: { produtoId: string; quantidade: number }) => ({
            produtoId: item.produtoId,
            quantidade: item.quantidade,
          }))
        );
      }
    } catch (error) {
      console.error('Erro ao carregar:', error);
    } finally {
      setCarregando(false);
    }
  }, [id]);

  useEffect(() => {
    loadData().then(() => {});
  }, [loadData]);

  const addItem = () => {
    setItens([...itens, { produtoId: '', quantidade: 1 }]);
  };

  const updateItem = (index: number, field: string, value: string | number) => {
    const newItens = [...itens];
    newItens[index] = { ...newItens[index], [field]: value };
    setItens(newItens);
  };

  const removeItem = (index: number) => {
    setItens(itens.filter((_, i) => i !== index));
  };

  const getItemTotal = (produtoId: string, quantidade: number) => {
    const produto = produtos.find((p) => p.id === produtoId);
    return produto ? produto.preco * quantidade : 0;
  };

  const getTotal = () => {
    return itens.reduce((acc, item) => acc + getItemTotal(item.produtoId, item.quantidade), 0);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!id || !clienteId) return;
    setLoading(true);
    try {
      const data: CreatePedidoRequest = {
        clienteId,
        itens: itens.filter((item) => item.produtoId),
      };
      await pedidos.update(id, data);
      navigate('/pedidos');
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
          <CardTitle>Editar Pedido</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="cliente">Cliente</Label>
              <Select value={clienteId} onValueChange={(v) => setClienteId(v || '')}>
                <SelectTrigger>
                  <SelectValue
                    placeholder="Selecione um cliente"
                    displayValue={
                      clienteId
                        ? clientes.find((c) => c.id === clienteId)?.nome
                        : undefined
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {clientes.map((c) => (
                    <SelectItem key={c.id} value={c.id}>
                      {c.nome}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <Select value={status} onValueChange={(v) => setStatus(v || '')}>
                <SelectTrigger>
                  <SelectValue placeholder="Selecione status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Pendente">Pendente</SelectItem>
                  <SelectItem value="Pago">Pago</SelectItem>
                  <SelectItem value="Cancelado">Cancelado</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <Label>Itens</Label>
                <Button type="button" variant="outline" size="sm" onClick={addItem}>
                  <Plus className="mr-2 h-4 w-4" />
                  Adicionar Item
                </Button>
              </div>
              <div className="space-y-2">
                {itens.map((item, index) => (
                  <div key={index} className="flex gap-2 items-end">
                    <div className="flex-1">
                      <Select
                        value={item.produtoId}
                        onValueChange={(v) => updateItem(index, 'produtoId', v || '')}
                      >
                        <SelectTrigger>
                           <SelectValue
                            placeholder="Selecione produto"
                            displayValue={
                              item.produtoId
                                ? produtos.find((p) => p.id === item.produtoId)?.nome
                                : undefined
                            }
                          />
                        </SelectTrigger>
                        <SelectContent>
                          {produtos
                            .filter((p) => p.ativo)
                            .map((p) => (
                              <SelectItem key={p.id} value={p.id}>
                                {p.nome} - R$ {p.preco.toFixed(2)}
                              </SelectItem>
                            ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="w-24">
                      <Input
                        type="number"
                        min="1"
                        value={item.quantidade}
                        onChange={(e) =>
                          updateItem(index, 'quantidade', parseInt(e.target.value) || 1)
                        }
                      />
                    </div>
                    <div className="w-28 text-right">
                      R$ {getItemTotal(item.produtoId, item.quantidade).toFixed(2)}
                    </div>
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon-sm"
                      onClick={() => removeItem(index)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                ))}
              </div>
            </div>

            <div className="flex justify-end text-lg font-bold">
              Total: R$ {getTotal().toFixed(2)}
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={loading}>
                {loading ? 'Salvando...' : 'Salvar'}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate('/pedidos')}>
                Cancelar
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}