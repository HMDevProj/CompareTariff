using Shared.Models;

namespace Data.Interfaces
{
    public interface ITariffRepository
    {
        Task<IEnumerable<Tariff>> GetAllTariffsAsync();
        Task AddTariffAsync(Tariff tariff);
        Task<Tariff?> GetTariffByIdAsync(int id);
        Task UpdateTariffAsync(Tariff tariff);
        Task DeleteTariffAsync(int id);
    }
}

