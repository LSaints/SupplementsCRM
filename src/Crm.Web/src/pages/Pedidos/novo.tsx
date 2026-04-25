import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Dialog, DialogContent } from '@/components/ui/dialog';
import { pedidos, type CreatePedidoRequest } from '@/services/api/pedidos';
import { clientes as clientesApi } from '@/services/api/clientes';
import { produtos as produtosApi } from '@/services/api/produtos';
import type { Cliente, Produto } from '@/services/types';
import { Trash2, Plus, Search, User, Package } from 'lucide-react';

export function NovoPedido() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [clienteId, setClienteId] = useState('');
  const [clienteBusca, setClienteBusca] = useState('');
  const [clienteDialogOpen, setClienteDialogOpen] = useState(false);
  const [itens, setItens] = useState<{ produtoId: string; quantidade: number; produtoBusca: string }[]>([]);

  const loadData = useCallback(async () => {
    try {
      const [clientesData, produtosData] = await Promise.all([
        clientesApi.getAll(),
        produtosApi.getAll(true),
      ]);
      setClientes(clientesData);
      setProdutos(produtosData);
    } catch (error) {
      console.error('Erro ao carregar:', error);
    }
  }, []);

  useEffect(() => {
    loadData().then(() => {});
  }, [loadData]);

  const addItem = () => {
    setItens([...itens, { produtoId: '', quantidade: 1, produtoBusca: '' }]);
  };

  const updateItem = (index: number, field: string, value: string | number) => {
    setItens((prev) => {
      const newItens = [...prev];
      newItens[index] = { ...newItens[index], [field]: value };
      return newItens;
    });
  };

  const removeItem = (index: number) => {
    setItens((prev) => prev.filter((_, i) => i !== index));
  };

  const getItemTotal = (produtoId: string, quantidade: number) => {
    const produto = produtos.find((p) => p.id === produtoId);
    return produto ? produto.preco * quantidade : 0;
  };

  const filteredClientes = clienteBusca.length > 0
    ? clientes.filter((c) => {
        const term = clienteBusca.toLowerCase();
        return (
          c.nome?.toLowerCase().includes(term) ||
          c.email?.toLowerCase().includes(term) ||
          c.telefone?.toLowerCase().includes(term)
        );
      })
    : [];

  const selectCliente = (cliente: Cliente) => {
    setClienteId(cliente.id);
    setClienteBusca(cliente.nome || '');
    setClienteDialogOpen(false);
  };

  const getTotal = () => {
    return itens.reduce((acc, item) => acc + getItemTotal(item.produtoId, item.quantidade), 0);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!clienteId || itens.length === 0) return;
    setLoading(true);
    try {
      const data: CreatePedidoRequest = {
        clienteId,
        itens: itens.filter((item) => item.produtoId).map((item) => ({
          produtoId: item.produtoId,
          quantidade: item.quantidade,
        })),
      };
      await pedidos.create(data);
      navigate('/pedidos');
    } catch (error) {
      console.error('Erro ao criar:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-xl">
      <Card>
        <CardHeader>
          <CardTitle>Novo Pedido</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="cliente">Cliente</Label>
              <div className="flex gap-2">
                <div className="flex-1">
                  <input
                    id="cliente"
                    type="text"
                    placeholder="Buscar cliente..."
                    className="h-8 w-full min-w-0 rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
                    value={clienteBusca}
                    onChange={(e) => {
                      setClienteBusca(e.target.value);
                      setClienteId('');
                    }}
                    onFocus={() => setClienteDialogOpen(true)}
                  />
                </div>
                {clienteId && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon-sm"
                    onClick={() => {
                      setClienteId('');
                      setClienteBusca('');
                    }}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                )}
              </div>
              <Dialog open={clienteDialogOpen} onOpenChange={setClienteDialogOpen}>
                <DialogContent className="max-w-md max-h-[60vh] overflow-y-auto">
                  <div className="space-y-4">
                    <div className="relative">
                      <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground pointer-events-none" />
                      <input
                        type="text"
                        placeholder="Buscar por nome, email ou telefone..."
                        className="h-8 w-full min-w-0 rounded-lg border border-input bg-transparent pl-8 pr-2.5 py-1 text-base transition-colors outline-none placeholder:text-muted-foreground"
                        value={clienteBusca}
                        onChange={(e) => setClienteBusca(e.target.value)}
                        autoFocus
                      />
                    </div>
                    <div className="space-y-1">
                      {filteredClientes.length === 0 ? (
                        <p className="text-sm text-muted-foreground py-4 text-center">
                          Nenhum cliente encontrado
                        </p>
                      ) : (
                        filteredClientes.map((c) => (
                          <button
                            key={c.id}
                            type="button"
                            className="w-full flex items-center gap-3 p-2 rounded-lg hover:bg-muted text-left"
                            onClick={() => selectCliente(c)}
                          >
                            <User className="h-8 w-8 text-muted-foreground" />
                            <div>
                              <div className="font-medium">{c.nome}</div>
                              <div className="text-xs text-muted-foreground">
                                {c.email} {c.telefone && `· ${c.telefone}`}
                              </div>
                            </div>
                          </button>
                        ))
                      )}
                    </div>
                  </div>
                </DialogContent>
              </Dialog>
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
                  <ItemRow
                    key={index}
                    item={item}
                    index={index}
                    produtos={produtos}
                    onUpdate={updateItem}
                    onRemove={removeItem}
                    getItemTotal={getItemTotal}
                  />
                ))}
              </div>
            </div>

            <div className="flex justify-end text-lg font-bold">
              Total: R$ {getTotal().toFixed(2)}
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={loading || !clienteId || itens.length === 0}>
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

function ItemRow({
  item,
  index,
  produtos,
  onUpdate,
  onRemove,
  getItemTotal,
}: {
  item: { produtoId: string; quantidade: number; produtoBusca: string };
  index: number;
  produtos: Produto[];
  onUpdate: (index: number, field: string, value: string | number) => void;
  onRemove: (index: number) => void;
  getItemTotal: (produtoId: string, quantidade: number) => number;
}) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [busca, setBusca] = useState('');

  const filteredProdutos = produtos.filter((p) => {
    if (!p.ativo) return false;
    const term = busca.toLowerCase();
    return p.nome.toLowerCase().includes(term) || p.descricao?.toLowerCase().includes(term);
  }).slice(0, 10);

  const selectProduto = (produto: Produto) => {
    onUpdate(index, 'produtoId', produto.id);
    onUpdate(index, 'produtoBusca', produto.nome);
    setBusca('');
    setDialogOpen(false);
  };

  const displayValue = item.produtoId
    ? item.produtoBusca || produtos.find(p => p.id === item.produtoId)?.nome || ''
    : '';

  return (
    <div className="flex gap-2 items-end">
      <div className="flex-1">
        <div className="flex gap-2">
          <div className="flex-1 relative">
            <input
              type="text"
              placeholder="Buscar produto..."
              className="h-8 w-full min-w-0 rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none placeholder:text-muted-foreground"
              value={displayValue}
              onChange={(e) => {
                if (item.produtoId) {
                  onUpdate(index, 'produtoId', '');
                  onUpdate(index, 'produtoBusca', '');
                }
                setBusca(e.target.value);
              }}
              onFocus={() => setDialogOpen(true)}
            />
          </div>
          {item.produtoId && (
            <Button
              type="button"
              variant="ghost"
              size="icon-sm"
              onClick={() => {
                onUpdate(index, 'produtoId', '');
                onUpdate(index, 'produtoBusca', '');
              }}
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          )}
        </div>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogContent className="max-w-md max-h-[60vh] overflow-y-auto">
            <div className="space-y-4">
              <div className="relative">
                <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground pointer-events-none" />
                <input
                  type="text"
                  placeholder="Buscar por nome ou descrição..."
                  className="h-8 w-full min-w-0 rounded-lg border border-input bg-transparent pl-8 pr-2.5 py-1 text-base transition-colors outline-none placeholder:text-muted-foreground"
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  autoFocus
                />
              </div>
              <div className="space-y-1">
                {filteredProdutos.length === 0 ? (
                  <p className="text-sm text-muted-foreground py-4 text-center">
                    Nenhum produto encontrado
                  </p>
                ) : (
                  filteredProdutos.map((p) => (
                    <button
                      key={p.id}
                      type="button"
                      className="w-full flex items-center gap-3 p-2 rounded-lg hover:bg-muted text-left"
                      onClick={() => selectProduto(p)}
                    >
                      <Package className="h-8 w-8 text-muted-foreground" />
                      <div>
                        <div className="font-medium">{p.nome}</div>
                        <div className="text-xs text-muted-foreground">
                          R$ {p.preco.toFixed(2)}
                          {p.descricao && ` · ${p.descricao}`}
                        </div>
                      </div>
                    </button>
                  ))
                )}
              </div>
            </div>
          </DialogContent>
        </Dialog>
      </div>
      <div className="w-24">
        <input
          type="number"
          min="1"
          className="h-8 w-full min-w-0 rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none"
          value={item.quantidade}
          onChange={(e) =>
            onUpdate(index, 'quantidade', parseInt(e.target.value) || 1)
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
        onClick={() => onRemove(index)}
      >
        <Trash2 className="h-4 w-4" />
      </Button>
    </div>
  );
}