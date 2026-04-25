import { vi } from 'vitest';
import type { Usuario, Produto, Cliente, Pedido } from '@/services/types';

export const mockUsuario: Usuario = {
  id: '123',
  nome: 'Test User',
  email: 'test@example.com',
  role: 'Admin',
};

export const mockProduto: Produto = {
  id: 'prod-1',
  nome: 'Produto Teste',
  descricao: 'Descrição do produto',
  preco: 100.00,
  ativo: true,
};

export const mockCliente: Cliente = {
  id: 'cli-1',
  nome: 'Cliente Teste',
  email: 'cliente@teste.com',
  telefone: '(11) 99999-9999',
  vendedorId: '123',
  vendedorNome: 'Test User',
};

export const mockPedido: Pedido = {
  id: 'ped-1',
  clienteId: 'cli-1',
  clienteNome: 'Cliente Teste',
  valorTotal: 150.00,
  status: 'Pendente',
  itens: [],
};

export function createMockAuthService() {
  return {
    login: vi.fn().mockResolvedValue({
      token: 'mock-token',
      usuario: mockUsuario,
    }),
  };
}

export function createMockProdutosService(overrides?: Partial<ReturnType<typeof vi.fn>>) {
  return {
    getAll: vi.fn().mockResolvedValue([mockProduto]),
    getById: vi.fn().mockResolvedValue(mockProduto),
    create: vi.fn().mockResolvedValue(mockProduto),
    update: vi.fn().mockResolvedValue(mockProduto),
    delete: vi.fn().mockResolvedValue(undefined),
    ...overrides,
  };
}

export function createMockClientesService() {
  return {
    getAll: vi.fn().mockResolvedValue([mockCliente]),
    getById: vi.fn().mockResolvedValue(mockCliente),
    create: vi.fn().mockResolvedValue(mockCliente),
    update: vi.fn().mockResolvedValue(mockCliente),
    delete: vi.fn().mockResolvedValue(undefined),
  };
}

export function createMockPedidosService() {
  return {
    getAll: vi.fn().mockResolvedValue([mockPedido]),
    getById: vi.fn().mockResolvedValue(mockPedido),
    create: vi.fn().mockResolvedValue(mockPedido),
    update: vi.fn().mockResolvedValue(mockPedido),
    updateStatus: vi.fn().mockResolvedValue(mockPedido),
    delete: vi.fn().mockResolvedValue(undefined),
  };
}

export function createErrorResponse(message: string, status: number = 400) {
  return {
    response: {
      status,
      data: { error: message },
    },
    isAxiosError: () => true,
    message: 'Request failed',
  };
}