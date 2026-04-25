import api from '@/lib/api';
import type { Produto } from '../types';

export const produtos = {
  getAll: async (incluirInativos = false): Promise<Produto[]> => {
    const response = await api.get<Produto[]>('/api/Produtos', {
      params: { incluirInativos },
    });
    return response.data;
  },

  getById: async (id: string): Promise<Produto> => {
    const response = await api.get<Produto>(`/api/Produtos/${id}`);
    return response.data;
  },

  create: async (data: Partial<Produto>): Promise<Produto> => {
    const response = await api.post<Produto>('/api/Produtos', data);
    return response.data;
  },

  update: async (id: string, data: Partial<Produto>): Promise<Produto> => {
    const response = await api.put<Produto>(`/api/Produtos/${id}`, data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/api/Produtos/${id}`);
  },
};