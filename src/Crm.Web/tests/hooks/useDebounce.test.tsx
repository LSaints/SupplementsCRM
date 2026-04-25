import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { useDebounce } from '@/hooks/useDebounce';

describe('useDebounce', () => {
  it('should return initial value immediately', () => {
    const TestComponent = ({ value, delay }: { value: string; delay: number }) => {
      const debounced = useDebounce(value, delay);
      return <div data-testid="debounced">{debounced}</div>;
    };

    render(<TestComponent value="initial" delay={500} />);

    expect(screen.getByTestId('debounced').textContent).toBe('initial');
  });

  it('should work with different types', () => {
    const TestComponent = ({ value, delay }: { value: number; delay: number }) => {
      const debounced = useDebounce(value, delay);
      return <div data-testid="debounced">{debounced}</div>;
    };

    render(<TestComponent value={100} delay={500} />);

    expect(screen.getByTestId('debounced').textContent).toBe('100');
  });

  it('should work with arrays', () => {
    const TestComponent = () => {
      const arr = useDebounce(['a', 'b'], 500);
      return <div data-testid="debounced">{arr.join(',')}</div>;
    };

    render(<TestComponent />);

    expect(screen.getByTestId('debounced').textContent).toBe('a,b');
  });

  it('should work with objects', () => {
    const TestComponent = () => {
      const obj = useDebounce({ key: 'value' }, 500);
      return <div data-testid="debounced">{obj.key}</div>;
    };

    render(<TestComponent />);

    expect(screen.getByTestId('debounced').textContent).toBe('value');
  });
});