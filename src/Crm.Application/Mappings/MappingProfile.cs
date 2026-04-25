using AutoMapper;
using Crm.Application.DTOs.Auth;
using Crm.Application.DTOs.Checkout;
using Crm.Application.DTOs.Cliente;
using Crm.Application.DTOs.Pedido;
using Crm.Application.DTOs.Produto;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Crm.Domain.ValueObjects;

namespace Crm.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Usuario, UsuarioDto>().ReverseMap();
        
        
        
        CreateMap<Cliente, ClienteDto>()
            .ForMember(d => d.VendedorNome, opt => opt.MapFrom(s => s.Vendedor != null ? s.Vendedor.Nome : null));
        
        CreateMap<ClienteDto, Cliente>();
        
        CreateMap<Produto, ProdutoDto>().ReverseMap();
        
        CreateMap<Pedido, PedidoDto>()
            .ForMember(d => d.ClienteNome, opt => opt.MapFrom(s => s.Cliente != null ? s.Cliente.Nome : null))
            .ForMember(d => d.VendedorNome, opt => opt.MapFrom(s => s.Vendedor != null ? s.Vendedor.Nome : null));
        
        CreateMap<PedidoDto, Pedido>();
        
        CreateMap<PedidoItem, PedidoItemDto>()
            .ForMember(d => d.ProdutoNome, opt => opt.MapFrom(s => s.Produto != null ? s.Produto.Nome : null));
        
        CreateMap<PedidoItemDto, PedidoItem>();
        
        CreateMap<LinkPagamento, LinkPagamentoDto>().ReverseMap();
        
        CreateMap<Endereco, EnderecoDto>().ReverseMap();
    }
}