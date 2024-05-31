using Business.Interfaces;
using Shared.Models;

namespace Tests
{
    public class MockTariffService : ITariffService
    {
        private readonly List<Tariff> _tariffs;

        public MockTariffService()
        {
            _tariffs = new List<Tariff>
            {
                new Tariff { Name = "Basic Electricity Tariff", BaseCost = 60m, AdditionalKwhCost = 0.22m, IncludedKwh = 0 },
                new Tariff { Name = "Packaged Tariff", BaseCost = 800m, AdditionalKwhCost = 0.30m, IncludedKwh = 4000 }
            };
        }

        public Task AddTariffAsync(Tariff tariff)
        {
            _tariffs.Add(tariff);
            return Task.CompletedTask;
        }

        public Task<Tariff?> GetTariffByIdAsync(int id)
        {
            return Task.FromResult(_tariffs.FirstOrDefault(t => t.Id == id));
        }

        public Task<IEnumerable<Tariff>> GetTariffsAsync()
        {
            return Task.FromResult<IEnumerable<Tariff>>(_tariffs);
        }

        public decimal CalculateAnnualCost(Tariff tariff, int consumption)
        {
            if (tariff.IncludedKwh == null || tariff.AdditionalKwhCost == null)
            {
                throw new InvalidOperationException("Tariff configuration is invalid");
            }

            if (tariff.IncludedKwh == 0)
            {
                // Basic Electricity Tariff logic
                var baseCostPerMonth = tariff.BaseCost / 12;
                var annualBaseCost = baseCostPerMonth * 12;
                var annualConsumptionCost = consumption * tariff.AdditionalKwhCost.Value;
                var annualCost = annualBaseCost + annualConsumptionCost;

                return annualCost;
            }
            else
            {
                // Packaged Tariff logic
                var baseCost = tariff.BaseCost;
                var additionalKwhThreshold = tariff.IncludedKwh.Value;
                var additionalKwhCost = tariff.AdditionalKwhCost.Value;

                if (consumption <= additionalKwhThreshold)
                {
                    return baseCost;
                }

                var additionalCost = (consumption - additionalKwhThreshold) * additionalKwhCost;
                var totalCost = baseCost + additionalCost;

                return totalCost;
            }
        }

        public async Task<IEnumerable<BestTariff>> GetTariffComparisonsAsync(int consumption)
        {
            var bestTariffs = _tariffs.Select(t => new BestTariff
            {
                Name = t.Name,
                AnnualCost = CalculateAnnualCost(t, consumption)
            }).OrderBy(t => t.AnnualCost).ToList();

            return await Task.FromResult(bestTariffs);
        }

        public IEnumerable<Tariff> GetTariffs()
        {
            return _tariffs;
        }

        public Task UpdateTariffAsync(Tariff tariff)
        {
            var existingTariff = _tariffs.FirstOrDefault(t => t.Id == tariff.Id);
            if (existingTariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            existingTariff.Name = tariff.Name;
            existingTariff.Type = tariff.Type;
            existingTariff.BaseCost = tariff.BaseCost;
            existingTariff.IncludedKwh = tariff.IncludedKwh;
            existingTariff.AdditionalKwhCost = tariff.AdditionalKwhCost;

            return Task.CompletedTask;
        }

        public Task DeleteTariffAsync(int id)
        {
            var existingTariff = _tariffs.FirstOrDefault(t => t.Id == id);
            if (existingTariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            _tariffs.Remove(existingTariff);
            return Task.CompletedTask;
        }
    }
}
