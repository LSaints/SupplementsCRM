using AutoMapper;
using Crm.Application.DTOs.Cliente;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using FluentResults;

namespace Crm.Application.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClienteService(
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<ClienteDto>>> GetAll()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return Result.Ok(_mapper.Map<List<ClienteDto>>(clientes));
    }

    public async Task<Result<List<ClienteDto>>> GetByVendedorId(Guid vendedorId)
    {
        var clientes = await _clienteRepository.GetByVendedorIdAsync(vendedorId);
        return Result.Ok(_mapper.Map<List<ClienteDto>>(clientes));
    }

    public async Task<Result<ClienteDto>> GetById(Guid id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        
        return cliente is not null
            ? Result.Ok(_mapper.Map<ClienteDto>(cliente))
            : Result.Fail(new Error($"Cliente {id} não encontrado."));
    }

    public async Task<Result<ClienteDto>> Create(CreateClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return Result.Fail(new Error("Nome é obrigatório."));

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone,
            Endereco = request.Endereco is not null
                ? new Domain.ValueObjects.Endereco
                {
                    Rua = request.Endereco.Rua,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cidade = request.Endereco.Cidade,
                    Estado = request.Endereco.Estado,
                    Cep = request.Endereco.Cep
                }
                : null,
            VendedorId = request.VendedorId,
            CriadoEm = DateTime.UtcNow
        };

        _clienteRepository.Add(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<ClienteDto>(cliente));
    }

    public async Task<Result<ClienteDto>> Update(Guid id, UpdateClienteRequest request)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return Result.Fail(new Error($"Cliente {id} não encontrado."));

        cliente.Nome = request.Nome;
        cliente.Email = request.Email;
        cliente.Telefone = request.Telefone;
        if (request.Endereco is not null)
        {
            cliente.Endereco = new Domain.ValueObjects.Endereco
            {
                Rua = request.Endereco.Rua,
                Numero = request.Endereco.Numero,
                Complemento = request.Endereco.Complemento,
                Bairro = request.Endereco.Bairro,
                Cidade = request.Endereco.Cidade,
                Estado = request.Endereco.Estado,
                Cep = request.Endereco.Cep
            };
        }
        cliente.VendedorId = request.VendedorId;

        _clienteRepository.Update(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(_mapper.Map<ClienteDto>(cliente));
    }

    public async Task<Result> Delete(Guid id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null)
            return Result.Fail(new Error($"Cliente {id} não encontrado."));

        _clienteRepository.Delete(cliente);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}