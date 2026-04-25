import api from '@/lib/api';
import type { Pedido } from '../types';

export interface CreatePedidoRequest {
  clienteId: string;
  vendedorId?: string;
  itens: { produtoId: string; quantidade: number }[];
}

export const pedidos = {
  getAll: async (): Promise<Pedido[]> => {
    const response = await api.get<Pedido[]>('/api/Pedidos');
    return response.data;
  },

  getByVendedorId: async (vendedorId: string): Promise<Pedido[]> => {
    const response = await api.get<Pedido[]>(`/api/Pedidos/vendedor/${vendedorId}`);
    return response.data;
  },

  getByClienteId: async (clienteId: string): Promise<Pedido[]> => {
    const response = await api.get<Pedido[]>(`/api/Pedidos/cliente/${clienteId}`);
    return response.data;
  },

  getById: async (id: string): Promise<Pedido> => {
    const response = await api.get<Pedido>(`/api/Pedidos/${id}`);
    return response.data;
  },

  create: async (data: CreatePedidoRequest): Promise<Pedido> => {
    const response = await api.post<Pedido>('/api/Pedidos', data);
    return response.data;
  },

  update: async (id: string, data: CreatePedidoRequest): Promise<Pedido> => {
    const response = await api.put<Pedido>(`/api/Pedidos/${id}`, data);
    return response.data;
  },

  updateStatus: async (id: string, status: string): Promise<Pedido> => {
    const response = await api.patch<Pedido>(`/api/Pedidos/${id}/status`, status);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/api/Pedidos/${id}`);
  },
};