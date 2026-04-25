# PRD - Sistema de CRM para Vendas

## 1. Visão do Produto

Sistema de CRM simples para gerenciamento de vendas com geração de links de pagamento. Permite que vendedores criem pedidos, gerem links de checkout e acompanhem vendas através de dashboard.

## 2. Stakeholders

- **Admin**: Gestão de produtos evisualização de métricas
- **Vendedor**: Criação de pedidos e links de pagamento

## 3. Entidades

### 3.1 Lead
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| nome | string | sim |
| telefone | string | sim |
| origem | enum | sim |
| criado_em | datetime | sim |
| convertido_em | datetime | não |

**origem**: instagram | whatsapp | indicação

### 3.2 Cliente
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| nome | string | sim |
| telefone | string | sim |
| email | string | não |
| criado_em | datetime | sim |

### 3.3 Produto
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| nome | string | sim |
| preco | decimal | sim |
| descricao | text | não |
| ativo | boolean | sim |

### 3.4 Pedido
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| cliente_id | uuid | sim |
| status | enum | sim |
| valor_total | decimal | sim |
| criado_em | datetime | sim |
| pago_em | datetime | não |

**status**: pendente | pago | cancelado | expirado

### 3.5 PedidoItem
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| pedido_id | uuid | sim |
| produto_id | uuid | sim |
| quantidade | integer | sim |
| preco | decimal | sim |

### 3.6 LinkPagamento
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| slug | string | sim |
| pedido_id | uuid | sim |
| criado_por | uuid | sim |
| expira_em | datetime | sim |
| status | string | sim |

### 3.7 Usuario
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | uuid | sim |
| nome | string | sim |
| senha | string | sim |
| eh_admin | boolean | sim |

## 4. Fluxo do Sistema

```
1. Cliente contacta vendedor (WhatsApp/Instagram/Indicação)
2. Vendedor registra Lead no CRM
3. Vendedor cria Pedido → seleciona Produtos → gera LinkPagamento
4. Sistema gera URL: crm.com/p/{slug}
5. Cliente acessa link →Visualiza produtos, preços → clica "Pagar"
6. Gateway processa pagamento
7. Webhook recebe confirmação:
   - atualiza pedido.status = "pago"
   - se cliente não existe → cria cliente
```

## 5. Funcionalidades do MVP

### 5.1 Autenticação
- Login com usuário/senha
- Sessão autenticada

### 5.2 CRUD Produtos
- Listar produtos ativos
- Criar produto
- Editar produto
- Desativar produto

### 5.3 Criar Pedidos
- Selecionar cliente (existente ou novo)
- Adicionar produtos com quantidade
- Calcular valor total

### 5.4 Gerar Link de Checkout
- Gerar slug único
- Definir data de expiração
- URL pública: crm.com/p/{slug}

### 5.5 Página de CheckoutPública
- Listar produtos do pedido
- Exibir valores
- Campo telefone do cliente
- Botão "Pagar" → redireciona para gateway

### 5.6 Webhook de Pagamento
- Receber callback do gateway
- Validar assinatura
- Atualizar status do pedido
- Criar cliente automaticamente se não existir

### 5.7 Dashboard
- Leads hoje
- Vendas hoje
- Faturamento hoje
- Ticket médio

## 6. Interface

### 6.1 Dashboard
- Cards: Leads hoje, Vendas hoje, Faturamento hoje, Ticket médio

### 6.2 Leads
- Lista: nome, telefone, origem, data, status (convertido/não convertido)

### 6.3 Clientes
- Lista: nome, telefone, total gasto, última compra

### 6.4 Pedidos
- Lista: pedido, cliente, valor_total, data, status

### 6.5 Produtos
- CRUD: nome, preco, descricao, ativo

### 6.6 Gerar Link
- Formulário: selecionar produtos + quantidade → gerar link

## 7. Métricas

- Ticket médio
- Vendas por vendedor
- Produtos mais vendidos
- Taxa de conversão de Leads

## 8. Requisitos Técnicos

- Backend: API REST
- Autenticação: Session/JWT
- Gateway de pagamento: Integration com gateway suportado
- Público: páginas HTML simples para checkout

## 9. Prioridades

### P0 (MVP)
1. Login
2. CRUD Produtos
3. Criar Pedidos
4. Gerar Link de Checkout
5. Página de Checkoutpública
6. Webhook de pagamento
7. Dashboard básica

### P1
1. CRUD Leads
2. CRUD Clientes
3. Lista de Pedidos
4. Métricas detalhadas

### P2
1. Histórico de pedidos por cliente
2. Relatórios
3. Notificações