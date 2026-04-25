using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Crm.Application.DTOs.Auth;
using Crm.Application.Services.Interfaces;
using Crm.Domain.Entities;
using Crm.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Crm.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest request)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
        
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return Result.Fail(new Error("Email ou senha inválidos."));

        var token = GenerateJwtToken(usuario);
        var response = new LoginResponse
        {
            Token = token,
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };

        return Result.Ok(response);
    }

    public async Task<Result<LoginResponse>> Register(LoginRequest request)
    {
        var existente = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (existente is not null)
            return Result.Fail(new Error("Email já cadastrado."));

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Email.Split('@')[0],
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Role = "Vendedor",
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _usuarioRepository.Add(usuario);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(usuario);
        var response = new LoginResponse
        {
            Token = token,
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };

        return Result.Ok(response);
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "Crm",
            audience: _configuration["Jwt:Audience"] ?? "Crm",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}