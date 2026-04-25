import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import userEvent from '@testing-library/user-event';
import { Login } from '@/pages/Login';
import * as authService from '@/services/api/auth';

vi.mock('@/services/api/auth', () => ({
  auth: {
    login: vi.fn(),
  },
}));

vi.mock('@/hooks/useAuth', () => ({
  useAuth: vi.fn(() => ({
    login: vi.fn(),
    logout: vi.fn(),
    token: null,
    usuario: null,
  })),
}));

function TestWrapper({ children }: { children: React.ReactNode }) {
  return <BrowserRouter>{children}</BrowserRouter>;
}

describe('Login', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders login form', () => {
    render(<Login />, { wrapper: TestWrapper });

    expect(screen.getByPlaceholderText('Email')).toBeInTheDocument();
    expect(screen.getByPlaceholderText('Senha')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Entrar' })).toBeInTheDocument();
  });

  it('shows validation required when submitting empty form', async () => {
    const user = userEvent.setup();
    render(<Login />, { wrapper: TestWrapper });

    await user.click(screen.getByRole('button', { name: 'Entrar' }));

    expect(screen.getByPlaceholderText('Email')).toBeRequired();
  });

  it('shows generic error when login fails without specific message', async () => {
    const user = userEvent.setup();

    vi.mocked(authService.auth.login).mockRejectedValue({
      response: {
        status: 401,
        data: { error: 'Email ou senha incorretos' },
      },
      isAxiosError: () => true,
    });

    render(<Login />, { wrapper: TestWrapper });

    await user.type(screen.getByPlaceholderText('Email'), 'wrong@test.com');
    await user.type(screen.getByPlaceholderText('Senha'), 'wrongpassword');
    await user.click(screen.getByRole('button', { name: 'Entrar' }));

    await waitFor(() => {
      expect(screen.getByText(/Erro/i)).toBeInTheDocument();
    });
  });

  it('shows loading state while submitting', async () => {
    const user = userEvent.setup();

    vi.mocked(authService.auth.login).mockImplementation(
      () => new Promise((resolve) => setTimeout(() => resolve({ token: 'test', usuario: { id: '1', nome: 'Test', email: 'test@test.com', role: 'Admin' } }), 100))
    );

    render(<Login />, { wrapper: TestWrapper });

    await user.type(screen.getByPlaceholderText('Email'), 'test@test.com');
    await user.type(screen.getByPlaceholderText('Senha'), 'password');

    await user.click(screen.getByRole('button', { name: 'Entrar' }));

    expect(screen.getByRole('button', { name: 'Entrando...' })).toBeDisabled();
  });
});