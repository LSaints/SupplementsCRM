import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { DashboardLayout } from '@/layouts/DashboardLayout';
import { Login } from '@/pages/Login';
import { Dashboard } from '@/pages/Dashboard';
import { ProdutosIndex } from '@/pages/Produtos/index';
import { NovoProduto } from '@/pages/Produtos/novo';
import { EditarProduto } from '@/pages/Produtos/[id]';
import { PedidosIndex } from '@/pages/Pedidos/index';
import { NovoPedido } from '@/pages/Pedidos/novo';
import { EditarPedido } from '@/pages/Pedidos/[id]';
import { ClientesIndex } from '@/pages/Clientes/index';
import { NovoCliente } from '@/pages/Clientes/novo';
import { EditarCliente } from '@/pages/Clientes/[id]';
import { Checkout } from '@/pages/Checkout/[id]';
import { Toaster } from '@/components/ui/sonner';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { token } = useAuth();
  if (!token) return <Navigate to="/login" replace />;
  return <DashboardLayout>{children}</DashboardLayout>;
}

export function App() {
  return (
    <BrowserRouter>
      <Toaster />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/produtos"
          element={
            <ProtectedRoute>
              <ProdutosIndex />
            </ProtectedRoute>
          }
        />
        <Route
          path="/produtos/novo"
          element={
            <ProtectedRoute>
              <NovoProduto />
            </ProtectedRoute>
          }
        />
        <Route
          path="/produtos/:id"
          element={
            <ProtectedRoute>
              <EditarProduto />
            </ProtectedRoute>
}
        />
        <Route
          path="/pedidos"
          element={
            <ProtectedRoute>
              <PedidosIndex />
            </ProtectedRoute>
          }
        />
        <Route
          path="/pedidos/novo"
          element={
            <ProtectedRoute>
              <NovoPedido />
            </ProtectedRoute>
          }
        />
        <Route
          path="/pedidos/:id"
          element={
            <ProtectedRoute>
              <EditarPedido />
            </ProtectedRoute>
          }
        />
        <Route
          path="/clientes"
          element={
            <ProtectedRoute>
              <ClientesIndex />
            </ProtectedRoute>
          }
        />
        <Route
          path="/clientes/novo"
          element={
            <ProtectedRoute>
              <NovoCliente />
            </ProtectedRoute>
          }
        />
        <Route
          path="/clientes/:id"
          element={
            <ProtectedRoute>
              <EditarCliente />
            </ProtectedRoute>
          }
        />
        <Route path="/checkout/:id" element={<Checkout />} />
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}