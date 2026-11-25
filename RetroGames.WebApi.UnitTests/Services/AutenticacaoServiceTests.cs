
using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Model.Constantes;
using RetroGames.Repository;

namespace RetroGames.WebApi.UnitTests.Services;

public class AutenticacaoServiceTests
{
    [Fact]
    public async Task CadastrarCliente_EmailExistente_LancaErro106ENaoInsere()
    {
        // Arrange — Preparar o cenário
        var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        var repositorySetMock = new Mock<IRepositorySet>();
        var passwordHasherMock = new Mock<IPasswordHasher<Usuario>>();

        usuarioRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorEmailAsync(
                "cliente@exemplo.com"))
            .ReturnsAsync(new Usuario
            {
                Id = 1,
                Email = "cliente@exemplo.com",
                Ativo = true
            });

        repositorySetMock
            .Setup(repositorios => repositorios.Usuario)
            .Returns(usuarioRepositoryMock.Object);

        var servico = new AutenticacaoService(
            repositorySetMock.Object,
            passwordHasherMock.Object
        );

        // Act — Executar o método
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.CadastrarClienteAsync(
                "Cliente Teste",
                " cliente@exemplo.com ",
                "Teste123456"
            )
        );

        // Assert — Conferir o resultado
        Assert.Equal(106, excecao.Erro.Code);

        usuarioRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Usuario>()),
            Times.Never
        );
    }


    [Fact]
    public async Task CadastrarCliente_DadosValidos_CriaClienteComSenhaHash()
    {
        // Arrange — Preparar o cenário
        var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        var repositorySetMock = new Mock<IRepositorySet>();

        var passwordHasher = new PasswordHasher<Usuario>();

        usuarioRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorEmailAsync(
                "cliente@exemplo.com"))
            .ReturnsAsync((Usuario?)null);

        usuarioRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(
                It.IsAny<Usuario>()))
            .ReturnsAsync(42);

        repositorySetMock
            .Setup(repositorios => repositorios.Usuario)
            .Returns(usuarioRepositoryMock.Object);

        var servico = new AutenticacaoService(
            repositorySetMock.Object,
            passwordHasher
        );

        // Act — Executar o cadastro
        var cliente = await servico.CadastrarClienteAsync(
            "Cliente Teste",
            "cliente@exemplo.com",
            "Teste123456"
        );

        // Assert — Conferir os resultados
        Assert.Equal(42, cliente.Id);
        Assert.Equal("Cliente Teste", cliente.Nome);
        Assert.Equal("cliente@exemplo.com", cliente.Email);

        Assert.True(cliente.Ativo);
        Assert.Equal(PerfilIds.Cliente, cliente.IdPerfil);

        Assert.NotEmpty(cliente.SenhaHash);
        Assert.NotEqual("Teste123456", cliente.SenhaHash);

        var verificacao = passwordHasher.VerifyHashedPassword(
            cliente,
            cliente.SenhaHash,
            "Teste123456"
        );

        Assert.NotEqual(
            PasswordVerificationResult.Failed,
            verificacao
        );

        usuarioRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Usuario>()),
            Times.Once
        );
    }


    [Fact]
    public async Task CadastrarCliente_RepositoryLancaDuplicado_LancaErro106()
    {
        // Arrange — Preparar o cenário
        var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        var repositorySetMock = new Mock<IRepositorySet>();
        var passwordHasher = new PasswordHasher<Usuario>();

        usuarioRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorEmailAsync(
                "cliente@exemplo.com"))
            .ReturnsAsync((Usuario?)null);

        usuarioRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(
                It.IsAny<Usuario>()))
            .ThrowsAsync(
                new RegistroDuplicadoException(
                    new Exception("E-mail duplicado no banco.")
                )
            );

        repositorySetMock
            .Setup(repositorios => repositorios.Usuario)
            .Returns(usuarioRepositoryMock.Object);

        var servico = new AutenticacaoService(
            repositorySetMock.Object,
            passwordHasher
        );

        // Act — Tentar cadastrar o cliente
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.CadastrarClienteAsync(
                "Cliente Teste",
                "cliente@exemplo.com",
                "Teste123456"
            )
        );

        // Assert — Conferir o resultado
        Assert.Equal(
            BusinessErrors.EMAIL_JA_CADASTRADO.Code,
            excecao.Erro.Code
        );

        usuarioRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Usuario>()),
            Times.Once
        );
    }


}
