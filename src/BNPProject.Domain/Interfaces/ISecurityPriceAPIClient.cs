namespace BNPProject.Domain.Interfaces;

public interface ISecurityPriceAPIClient
{
    Task<Decimal> GetPriceAsync(string ISIN);
}
