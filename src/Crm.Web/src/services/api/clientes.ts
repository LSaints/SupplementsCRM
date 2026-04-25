import api from '@/lib/api';
import type { Cliente } from '../types';

export const clientes = {
  getAll: async (): Promise<Cliente[]> => {
    const response = await api.get<Cliente[]>('/api/Clientes');
    return response.data;
  },

  getByVendedorId: async (vendedorId: string): Promise<Cliente[]> => {
    const response = await api.get<Cliente[]>(`/api/Clientes/vendedor/${vendedorId}`);
    return response.data;
  },

  getById: async (id: string): Promise<Cliente> => {
    const response = await api.get<Cliente>(`/api/Clientes/${id}`);
    return response.data;
  },

  create: async (data: Partial<Cliente>): Promise<Cliente> => {
    const response = await api.post<Cliente>('/api/Clientes', data);
    return response.data;
  },

  update: async (id: string, data: Partial<Cliente>): Promise<Cliente> => {
    const response = await api.put<Cliente>(`/api/Clientes/${id}`, data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/api/Clientes/${id}`);
  },
};