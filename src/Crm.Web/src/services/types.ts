export interface Usuario {
  id: string;
  nome: string;
  email: string;
  role: string;
}

export interface Produto {
  id: string;
  nome: string;
  descricao?: string;
  preco: number;
  ativo: boolean;
}



export interface Cliente {
  id: string;
  nome?: string;
  email?: string;
  telefone?: string;
  vendedorId?: string;
  vendedorNome?: string;
  criadoEm?: string;
}

export interface Pedido {
  id: string;
  clienteId: string;
  clienteNome?: string;
  valorTotal: number;
  status: string;
  itens: PedidoItem[];
}

export interface PedidoItem {
  produtoId: string;
  quantidade: number;
  precoUnitario: number;
}

export interface LinkPagamento {
  id: string;
  pedidoId: string;
  url: string;
  utilizado: boolean;
  expiresAt?: string;
}

export interface DashboardDados {
  totalClientes: number;
  totalPedidos: number;
  pedidosPagos: number;
  faturamentoTotal: number;
}