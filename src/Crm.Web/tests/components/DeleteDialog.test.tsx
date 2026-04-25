import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { DeleteDialog } from '@/components/ui/delete-dialog';

describe('DeleteDialog', () => {
  it('renders dialog when open', () => {
    render(
      <DeleteDialog
        open={true}
        onOpenChange={() => {}}
        onConfirm={() => {}}
        title="Excluir Item"
        description="Tem certeza que deseja excluir este item?"
      />
    );

    expect(screen.getByText('Excluir Item')).toBeInTheDocument();
    expect(screen.getByText('Tem certeza que deseja excluir este item?')).toBeInTheDocument();
  });

  it('does not render when closed', () => {
    render(
      <DeleteDialog
        open={false}
        onOpenChange={() => {}}
        onConfirm={() => {}}
        title="Excluir Item"
        description="Tem certeza?"
      />
    );

    expect(screen.queryByText('Excluir Item')).not.toBeInTheDocument();
  });

  it('calls onOpenChange when cancel is clicked', async () => {
    const user = userEvent.setup();
    const onOpenChange = vi.fn();
    
    render(
      <DeleteDialog
        open={true}
        onOpenChange={onOpenChange}
        onConfirm={() => {}}
        title="Excluir Item"
        description="Tem certeza?"
      />
    );

    await user.click(screen.getByText('Cancelar'));
    
    expect(onOpenChange).toHaveBeenCalledWith(false);
  });

  it('calls onConfirm when delete is clicked', async () => {
    const user = userEvent.setup();
    const onConfirm = vi.fn();
    
    render(
      <DeleteDialog
        open={true}
        onOpenChange={() => {}}
        onConfirm={onConfirm}
        title="Excluir Item"
        description="Tem certeza?"
      />
    );

    await user.click(screen.getByText('Excluir'));
    
    expect(onConfirm).toHaveBeenCalledTimes(1);
  });

  it('shows loading state when deleting', () => {
    const onConfirm = vi.fn();
    
    render(
      <DeleteDialog
        open={true}
        onOpenChange={() => {}}
        onConfirm={onConfirm}
        title="Excluir Item"
        description="Tem certeza?"
        loading={true}
      />
    );

    expect(screen.getByText('Excluindo...')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Excluindo...' })).toBeDisabled();
  });

  it('disables button when loading', () => {
    const onConfirm = vi.fn();
    
    render(
      <DeleteDialog
        open={true}
        onOpenChange={() => {}}
        onConfirm={onConfirm}
        title="Excluir Item"
        description="Tem certeza?"
        loading={true}
      />
    );

    const button = screen.getByRole('button', { name: 'Excluindo...' });
    expect(button).toBeDisabled();
  });
});