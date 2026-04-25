import api from '@/lib/api';
import type { Usuario } from '../types';

interface LoginRequest {
  email: string;
  senha: string;
}

interface LoginResponse {
  token: string;
  usuario: Usuario;
}

export const auth = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/api/Auth/login', data);
    return response.data;
  },

  logout: async (): Promise<void> => {
    localStorage.removeItem('auth-storage');
  },
};