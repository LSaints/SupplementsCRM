import { ReactNode } from 'react';
import { BrowserRouter } from 'react-router-dom';

interface WrapperProps {
  children: ReactNode;
}

export function Wrappers({ children }: WrapperProps) {
  return <BrowserRouter>{children}</BrowserRouter>;
}