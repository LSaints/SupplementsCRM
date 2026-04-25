import { Link, useLocation } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { useAuth } from '@/hooks/useAuth';
import { LayoutDashboard, Package, Users, ShoppingCart, DollarSign } from 'lucide-react';

const navItems = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/produtos', label: 'Produtos', icon: Package },
  { to: '/clientes', label: 'Clientes', icon: Users},
  // { to: '/leads', label: 'Leads', icon: Users },
  { to: '/pedidos', label: 'Pedidos', icon: ShoppingCart },
];

export function Sidebar() {
  const location = useLocation();
  const { usuario } = useAuth();

  return (
    <aside className="flex w-64 flex-col border-r bg-card">
      <nav className="flex-1 space-y-1 p-4">
        {navItems.map((item) => {
          const Icon = item.icon;
          const isActive = location.pathname.startsWith(item.to);
          return (
            <Link
              key={item.to}
              to={item.to}
              className={cn(
                'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                isActive
                  ? 'bg-primary text-primary-foreground'
                  : 'text-muted-foreground hover:bg-accent hover:text-foreground'
              )}
            >
              <Icon className="size-4" />
              {item.label}
            </Link>
          );
        })}
      </nav>
      {usuario?.role === 'Admin' && (
        <div className="border-t p-4">
          <Link
            to="/financeiro"
            className="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground"
          >
            <DollarSign className="size-4" />
            Financeiro
          </Link>
        </div>
      )}
    </aside>
  );
}