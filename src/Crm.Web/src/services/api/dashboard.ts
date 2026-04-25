import api from '@/lib/api';
import type { DashboardDados } from '../types';

export const dashboard = {
  getDados: async (): Promise<DashboardDados> => {
    const response = await api.get<DashboardDados>('/api/Dashboard');
    return response.data;
  },
};