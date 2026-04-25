import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  usuario: { id: string; nome: string; email: string; role: string } | null;
  login: (token: string, usuario: AuthState['usuario']) => void;
  logout: () => void;
}

export const useAuth = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      usuario: null,
      login: (token, usuario) => set({ token, usuario }),
      logout: () => set({ token: null, usuario: null }),
    }),
    { name: 'auth-storage' }
  )
);