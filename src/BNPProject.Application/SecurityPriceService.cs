using BNPProject.Application.Interfaces;
using BNPProject.Domain.DTOs;
using BNPProject.Domain.Interfaces;

namespace BNPProject.Application;

public  class SecurityPriceService(ISecurityPriceAPIClient securityPriceAPIClient, ISecurityPriceRepository securityPriceRepository) : ISecurityPriceService
{
    private readonly ISecurityPriceAPIClient _securityPriceAPIClient = securityPriceAPIClient;
    private readonly ISecurityPriceRepository _securityPriceRepository = securityPriceRepository;

    public async Task RetrieveAndStorePricesAsync(IEnumerable<string> ISINS)
    {
        foreach(var isin in ISINS)
        {
            if (isin.Length != 12)
                throw new ArgumentException("Invalid ISIN length");

            var price = await _securityPriceAPIClient.GetPriceAsync(isin);
            var securityPrice = new SecurityPrice { ISIN = isin,  Price = price };
            await _securityPriceRepository.StorePriceAsync(securityPrice);
        }
    }
}
