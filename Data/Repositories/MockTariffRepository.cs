using Data.Interfaces;
using Exceptions;
using Shared.Models;

namespace Data.Repositories
{
    public class MockTariffRepository : ITariffRepository
    {
        private readonly List<Tariff> _tariffs = new List<Tariff>
        {
            new Tariff { Id = 1, Name = "Product A", Type = 1, BaseCost = 100m, IncludedKwh = 1000, AdditionalKwhCost = 0.3m },
            new Tariff { Id = 2, Name = "Product B", Type = 2, BaseCost = 200m, IncludedKwh = 2000, AdditionalKwhCost = 0.25m }
        };

        public Task<IEnumerable<Tariff>> GetAllTariffsAsync()
        {
            return Task.FromResult(_tariffs.AsEnumerable());
        }

        public async Task AddTariffAsync(Tariff tariff)
        {
            var existingTariff = await GetTariffByIdAsync(tariff.Id);

            if (existingTariff != null)
            {
                throw new DuplicateTariffException("A tariff with the same Id already exists.");
            }

            var existingTariffByName = _tariffs.FirstOrDefault(t => t.Name == tariff.Name);
            if (existingTariffByName != null)
            {
                throw new DuplicateTariffException("A tariff with the same Name already exists.");
            }

            _tariffs.Add(tariff);
        }

        public Task<Tariff?> GetTariffByIdAsync(int id)
        {
            var tariff = _tariffs.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(tariff);
        }

        public async Task UpdateTariffAsync(Tariff tariff)
        {
            var existingTariff = await GetTariffByIdAsync(tariff.Id);
            if (existingTariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            existingTariff.Name = tariff.Name;
            existingTariff.Type = tariff.Type;
            existingTariff.BaseCost = tariff.BaseCost;
            existingTariff.IncludedKwh = tariff.IncludedKwh;
            existingTariff.AdditionalKwhCost = tariff.AdditionalKwhCost;
        }

        public async Task DeleteTariffAsync(int id)
        {
            var existingTariff = await GetTariffByIdAsync(id);
            if (existingTariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            _tariffs.Remove(existingTariff);
        }
    }
}
