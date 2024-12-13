using BNPProject.Domain.DTOs;
using BNPProject.Domain.Interfaces;
using Moq;

namespace BNPProject.Application.Test;

public class SecurityPriceServiceTests
{
    private readonly Mock<ISecurityPriceAPIClient> _apiClientMock;
    private readonly Mock<ISecurityPriceRepository> _repositoryMock;
    private readonly SecurityPriceService _securityPriceService;

    public SecurityPriceServiceTests()
    {
        _apiClientMock = new Mock<ISecurityPriceAPIClient>();
        _repositoryMock = new Mock<ISecurityPriceRepository>();
        _securityPriceService = new SecurityPriceService(_apiClientMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task RetrieveAndStorePricesAsync_ShouldRetrieveAndStorePrices()
    {
        // Arrange
        var isins = new List<string> { "BNP123456789", "BNP987654321" };
        _apiClientMock.Setup(client => client.GetPriceAsync(It.IsAny<string>())).ReturnsAsync(100m);

        // Act
        await _securityPriceService.RetrieveAndStorePricesAsync(isins);
        
        // Assert
        foreach (var isin in isins)
        {
            _apiClientMock.Verify(client => client.GetPriceAsync(isin), Times.Once);
            _repositoryMock.Verify(repo => repo.StorePriceAsync(It.Is<SecurityPrice>(sp => sp.ISIN == isin && sp.Price == 100m)), Times.Once);
        }
    }

    [Fact]
    public async Task RetrieveAndStorePricesAsync_ShouldHandleEmptyList()
    {
        // Arrange
        var isins = new List<string>();

        // Act
        await _securityPriceService.RetrieveAndStorePricesAsync(isins);

        // Assert
        _apiClientMock.Verify(client => client.GetPriceAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(repo => repo.StorePriceAsync(It.IsAny<SecurityPrice>()), Times.Never);
    }

    [Fact]
    public async Task RetrieveAndStorePricesAsync_ShouldHandleApiException()
    {
        // Arrange
        var isins = new List<string> { "BNP123456789" };
        _apiClientMock.Setup(client => client.GetPriceAsync(It.IsAny<string>())).ThrowsAsync(new HttpRequestException("API failure"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () => await _securityPriceService.RetrieveAndStorePricesAsync(isins));

        _repositoryMock.Verify(repo => repo.StorePriceAsync(It.IsAny<SecurityPrice>()), Times.Never);
    }

    [Fact]
    public async Task RetrieveAndStorePricesAsync_ShouldThrowExceptionForInvalidISINLength()
    {
        // Arrange
        var isins = new List<string> { "INVALIDISIN" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _securityPriceService.RetrieveAndStorePricesAsync(isins));

        Assert.Contains("Invalid ISIN length", exception.Message);
        _apiClientMock.Verify(client => client.GetPriceAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(repo => repo.StorePriceAsync(It.IsAny<SecurityPrice>()), Times.Never);
    }
}
