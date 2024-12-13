using BNPProject.Domain.DTOs;

namespace BNPProject.Domain.Interfaces;

public interface ISecurityPriceRepository
{
    Task StorePriceAsync(SecurityPrice securityPrice);
}
