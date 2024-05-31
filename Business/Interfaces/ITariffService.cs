using Shared.Models;

namespace Business.Interfaces
{
    public interface ITariffService
    {
        Task<IEnumerable<Tariff>> GetTariffsAsync();
        Task AddTariffAsync(Tariff tariff);
        Task<Tariff?> GetTariffByIdAsync(int id);
        Task<IEnumerable<BestTariff>> GetTariffComparisonsAsync(int consumption);
        IEnumerable<Tariff> GetTariffs();
        decimal CalculateAnnualCost(Tariff tariff, int consumption);
        Task UpdateTariffAsync(Tariff tariff);
        Task DeleteTariffAsync(int id);
    }
}
