namespace BNPProject.Application.Interfaces;

internal interface ISecurityPriceService
{
    Task RetrieveAndStorePricesAsync(IEnumerable<string> ISINS);
}
