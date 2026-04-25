import { type ReactNode } from 'react';
import { Outlet } from 'react-router-dom';
import { Header } from '@/components/layout/Header';
import { Sidebar } from '@/components/layout/Sidebar';

interface DashboardLayoutProps {
  children?: ReactNode;
}

export function DashboardLayout({ children }: DashboardLayoutProps) {
  return (
    <div className="flex h-screen flex-col">
      <Header />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        <main className="flex-1 overflow-y-auto bg-background p-6">
          {children ?? <Outlet />}
        </main>
      </div>
    </div>
  );
}