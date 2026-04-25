import { ReactElement, ReactNode } from 'react';
import { render as rtlRender, type RenderOptions } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';

interface WrapperProps {
  children: ReactNode;
}

function Wrappers({ children }: WrapperProps) {
  return <BrowserRouter>{children}</BrowserRouter>;
}

function customRender(
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) {
  return rtlRender(ui, { wrapper: Wrappers, ...options });
}

export { customRender as render };