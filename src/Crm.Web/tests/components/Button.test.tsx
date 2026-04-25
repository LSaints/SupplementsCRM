import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Button } from '@/components/ui/button';

describe('Button', () => {
  it('renders with default variant and size', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByRole('button')).toBeInTheDocument();
    expect(screen.getByText('Click me')).toBeInTheDocument();
  });

  it('renders with different variants', () => {
    const { container } = render(
      <>
        <Button variant="default">Default</Button>
        <Button variant="outline">Outline</Button>
        <Button variant="secondary">Secondary</Button>
        <Button variant="ghost">Ghost</Button>
        <Button variant="destructive">Destructive</Button>
        <Button variant="link">Link</Button>
      </>
    );
    
    expect(container).toBeInTheDocument();
  });

  it('renders with different sizes', () => {
    const { container } = render(
      <>
        <Button size="default">Default</Button>
        <Button size="sm">Small</Button>
        <Button size="lg">Large</Button>
        <Button size="icon">Icon</Button>
      </>
    );
    
    expect(container).toBeInTheDocument();
  });

  it('handles click events', async () => {
    const user = userEvent.setup();
    const handleClick = vi.fn();
    
    render(<Button onClick={handleClick}>Click me</Button>);
    
    await user.click(screen.getByRole('button'));
    
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('can be disabled', () => {
    render(<Button disabled>Disabled</Button>);
    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('shows loading state', () => {
    render(<Button disabled>Loading...</Button>);
    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('renders as child when asChild is used', () => {
    render(
      <Button asChild>
        <a href="/test">Link Button</a>
      </Button>
    );
    
    expect(screen.getByRole('link')).toBeInTheDocument();
    expect(screen.getByRole('link')).toHaveTextContent('Link Button');
  });
});