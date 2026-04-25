import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { checkout } from '@/services/api/checkout';
import type { Pedido } from '@/services/types';
import { CheckCircle, XCircle, Loader2, CreditCard } from 'lucide-react';

export function Checkout() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [pagando, setPagando] = useState(false);
  const [pedido, setPedido] = useState<Pedido | null>(null);
  const [erro, setErro] = useState('');
  const [sucesso, setSucesso] = useState(false);

  useEffect(() => {
    loadData();
  }, [id]);

  const loadData = async () => {
    if (!id) return;
    try {
      const pedidoData = await checkout.getCheckoutData(id);
      setPedido(pedidoData);
    } catch {
      setErro('Link de pagamento não encontrado ou expirado.');
    } finally {
      setLoading(false);
    }
  };

  const handlePagar = async () => {
    if (!id) return;
    setPagando(true);
    setErro('');

    try {
      await checkout.confirmarPagamento(id);
      setSucesso(true);
    } catch {
      setErro('Erro ao processar pagamento. Tente novamente.');
    } finally {
      setPagando(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  if (erro) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-destructive">
              <XCircle className="h-5 w-5" />
              Erro
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-muted-foreground">{erro}</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (sucesso) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-green-600">
              <CheckCircle className="h-5 w-5" />
              Pagamento Realizado
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-muted-foreground">
              Seu pagamento foi processado com sucesso. Obrigado!
            </p>
            <Button variant="outline" onClick={() => navigate('/')}>
              Voltar
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (!pedido) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background">
        <div className="text-muted-foreground">Link inválido</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5" />
            Checkout
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Pedido:</span>
              <span className="font-medium">{pedido.id.slice(0, 8)}</span>
            </div>
            <div className="flex justify-between text-lg font-bold">
              <span>Total:</span>
              <span>R$ {pedido.valorTotal.toFixed(2)}</span>
            </div>
          </div>

          <Button className="w-full" size="lg" onClick={handlePagar} disabled={pagando}>
            {pagando ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Processando...
              </>
            ) : (
              <>
                <CreditCard className="mr-2 h-4 w-4" />
                Pagar Agora
              </>
            )}
          </Button>

          <p className="text-xs text-center text-muted-foreground">
            Este é um ambiente de teste. Clique em "Pagar Agora" para simular o pagamento.
          </p>
        </CardContent>
      </Card>
    </div>
  );
}