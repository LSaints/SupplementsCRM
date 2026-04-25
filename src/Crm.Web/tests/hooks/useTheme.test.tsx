import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { act, render, screen } from '@testing-library/react';
import { useTheme } from '@/hooks/useTheme';

describe('useTheme', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.stubGlobal('matchMedia', vi.fn().mockImplementation((query: string) => ({
      matches: query === '(prefers-color-scheme: dark)',
      media: query,
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })));
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it('should return light theme by default when not dark', async () => {
    vi.stubGlobal('matchMedia', vi.fn().mockReturnValue({
      matches: false,
      media: '',
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    }));

    const TestComponent = () => {
      const { theme } = useTheme();
      return <div data-testid="theme">{theme}</div>;
    };

    await act(async () => {
      render(<TestComponent />);
    });

    expect(screen.getByTestId('theme').textContent).toBe('light');
  });

  it('should toggle theme from light to dark', async () => {
    vi.stubGlobal('matchMedia', vi.fn().mockReturnValue({
      matches: false,
      media: '',
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    }));

    const TestComponent = () => {
      const { theme, toggleTheme } = useTheme();
      return (
        <div>
          <div data-testid="theme">{theme}</div>
          <button onClick={toggleTheme} data-testid="toggle">Toggle</button>
        </div>
      );
    };

    await act(async () => {
      render(<TestComponent />);
    });

    expect(screen.getByTestId('theme').textContent).toBe('light');

    await act(async () => {
      screen.getByTestId('toggle').click();
    });

    expect(screen.getByTestId('theme').textContent).toBe('dark');
  });
});