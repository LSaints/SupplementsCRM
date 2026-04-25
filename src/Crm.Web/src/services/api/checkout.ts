import api from '@/lib/api';
import type { LinkPagamento, Pedido } from '../types';

interface CreateLinkPagamentoRequest {
  pedidoId: string;
}

export const checkout = {
  criarLinkPagamento: async (data: CreateLinkPagamentoRequest): Promise<LinkPagamento> => {
    const response = await api.post<LinkPagamento>('/api/Checkout/link', data);
    return response.data;
  },

  getLinkPagamento: async (pedidoId: string): Promise<LinkPagamento> => {
    const response = await api.get<LinkPagamento>(`/api/Checkout/link/${pedidoId}`);
    return response.data;
  },

  getCheckoutData: async (pedidoId: string): Promise<Pedido> => {
    const response = await api.get<Pedido>(`/api/Checkout/${pedidoId}`);
    return response.data;
  },

  confirmarPagamento: async (pedidoId: string): Promise<void> => {
    await api.post(`/api/Checkout/link/${pedidoId}/confirmar`);
  },
};