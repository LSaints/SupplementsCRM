import { describe, it, expect, beforeEach } from 'vitest';
import { useAuth } from '@/hooks/useAuth';

describe('useAuth', () => {
  beforeEach(() => {
    useAuth.getState().logout();
  });

  it('initial state should have null token and usuario', () => {
    const { token, usuario } = useAuth.getState();
    expect(token).toBeNull();
    expect(usuario).toBeNull();
  });

  it('login should set token and usuario', () => {
    const { login } = useAuth.getState();
    const testToken = 'test-token-123';
    const testUsuario = {
      id: 'user-1',
      nome: 'Test User',
      email: 'test@test.com',
      role: 'Admin',
    };

    login(testToken, testUsuario);

    const { token, usuario } = useAuth.getState();
    expect(token).toBe(testToken);
    expect(usuario).toEqual(testUsuario);
  });

  it('logout should clear token and usuario', () => {
    const { login, logout } = useAuth.getState();
    login('test-token', { id: '1', nome: 'Test', email: 'test@test.com', role: 'Admin' });

    logout();

    const { token, usuario } = useAuth.getState();
    expect(token).toBeNull();
    expect(usuario).toBeNull();
  });

  it('should persist to localStorage', () => {
    const { login } = useAuth.getState();
    login('persist-token', { id: '1', nome: 'Test', email: 'test@test.com', role: 'Admin' });

    const stored = localStorage.getItem('auth-storage');
    expect(stored).toBeTruthy();
    const parsed = JSON.parse(stored!);
    expect(parsed.state.token).toBe('persist-token');
  });
});