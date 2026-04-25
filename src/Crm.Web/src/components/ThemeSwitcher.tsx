import { useTheme } from '@/hooks/useTheme';
import { Button } from '@/components/ui/button';
import { Moon, Sun } from 'lucide-react';

export function ThemeSwitcher() {
  const { theme, toggleTheme } = useTheme();

  return (
    <Button variant="ghost" size="icon-xs" onClick={toggleTheme}>
      {theme === 'light' ? (
        <Moon className="size-3.5" />
      ) : (
        <Sun className="size-3.5" />
      )}
    </Button>
  );
}