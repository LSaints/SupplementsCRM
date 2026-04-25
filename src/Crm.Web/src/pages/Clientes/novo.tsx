import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { clientes } from '@/services/api/clientes';

export function NovoCliente() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [telefone, setTelefone] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await clientes.create({
        nome,
        email,
        telefone,
      });
      navigate('/clientes');
    } catch (error) {
      console.error('Erro ao criar:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-xl">
      <Card>
        <CardHeader>
          <CardTitle>Novo Cliente</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="nome">Nome</Label>
              <Input
                id="nome"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </div>
            <div className="space-y-2">
                <Label htmlFor="telefone">Telefone</Label>
                <Input
                  id="telefone"
                  type="tel"
                  value={telefone}
                  onChange={(e) => setTelefone(e.target.value)}
                />
              </div>
            <div className="flex gap-2">
              <Button type="submit" disabled={loading}>
                {loading ? 'Salvando...' : 'Salvar'}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate('/clientes')}>
                Cancelar
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}