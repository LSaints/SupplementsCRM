import { ReactElement } from 'react';
import { render as rtlRender, type RenderOptions } from '@testing-library/react';
import { Wrappers } from './wrappers';

function customRender(
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) {
  return rtlRender(ui, { wrapper: Wrappers, ...options });
}

export { customRender as render };