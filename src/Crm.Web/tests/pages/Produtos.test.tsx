import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { ProdutosIndex } from '@/pages/Produtos/index';
import * as produtosService from '@/services/api/produtos';
import type { Produto } from '@/services/types';

const mockProdutos: Produto[] = [
  { id: '1', nome: 'Produto A', preco: 100.00, ativo: true },
  { id: '2', nome: 'Produto B', preco: 50.00, ativo: false },
];

vi.mock('@/services/api/produtos', () => ({
  produtos: {
    getAll: vi.fn(),
    delete: vi.fn(),
  },
}));

function TestWrapper({ children }: { children: React.ReactNode }) {
  return <BrowserRouter>{children}</BrowserRouter>;
}

describe('ProdutosIndex', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(produtosService.produtos.getAll).mockResolvedValue(mockProdutos);
    vi.mocked(produtosService.produtos.delete).mockResolvedValue(undefined);
  });

  it('renders loading state initially', () => {
    render(<ProdutosIndex />);
    expect(screen.getByText('Carregando...')).toBeInTheDocument();
  });

  it('renders produtos list after loading', async () => {
    render(<ProdutosIndex />, { wrapper: TestWrapper });

    await waitFor(() => {
      expect(screen.getByText('Produto A')).toBeInTheDocument();
    });
  });

  it('renders table headers', async () => {
    render(<ProdutosIndex />, { wrapper: TestWrapper });

    await waitFor(() => {
      expect(screen.getByText('Nome')).toBeInTheDocument();
    });
  });

  it('shows novo button', async () => {
    render(<ProdutosIndex />, { wrapper: TestWrapper });

    await waitFor(() => {
      expect(screen.getByRole('link', { name: 'Novo' })).toHaveAttribute('href', '/produtos/novo');
    });
  });

  it('displays active status badge', async () => {
    render(<ProdutosIndex />, { wrapper: TestWrapper });

    await waitFor(() => {
      expect(screen.getByText('Ativo')).toBeInTheDocument();
    });
  });

  it('formats price correctly', async () => {
    render(<ProdutosIndex />, { wrapper: TestWrapper });

    await waitFor(() => {
      expect(screen.getByText('R$ 100.00')).toBeInTheDocument();
    });
  });
});