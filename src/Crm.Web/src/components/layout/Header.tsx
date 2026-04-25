import { Link } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/button';
import { ThemeSwitcher } from '@/components/ThemeSwitcher';
import { LogOut, User } from 'lucide-react';

export function Header() {
  const { usuario, logout } = useAuth();

  return (
    <header className="h-16 bg-card border-b border-border flex items-center justify-between px-6">
      <Link to="/dashboard" className="text-xl font-bold">
        CRM
      </Link>

      <div className="flex items-center gap-4">
        <ThemeSwitcher />
        <div className="flex items-center gap-2">
          <User className="size-4 text-muted-foreground" />
          <span className="text-sm">{usuario?.nome}</span>
        </div>
        <Button variant="ghost" size="icon-xs" onClick={logout}>
          <LogOut className="size-4" />
        </Button>
      </div>
    </header>
  );
}