using BNPProject.Domain.Interfaces;

namespace BNPPProject.Controllers.External;

public class SecurityPriceAPIClient(HttpClient httpClient): ISecurityPriceAPIClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<decimal> GetPriceAsync(string ISIN)
    {
        var response = await _httpClient.GetAsync($"https://securities.dataprovider.com/securityprice/{ISIN}");
        response.EnsureSuccessStatusCode();

        var price = await response.Content.ReadAsStringAsync();
        return decimal.Parse( price );
    }
}
