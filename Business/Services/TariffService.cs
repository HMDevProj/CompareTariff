using Business.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Shared.Models;

namespace Business.Services
{
    public class TariffService : ITariffService
    {
        private readonly ITariffRepository _tariffRepository;
        private readonly IMemoryCache _memoryCache;
        private const string TariffsCacheKey = "TariffsCacheKey";

        private List<Tariff> _tariffs;

        public TariffService(ITariffRepository tariffRepository, IMemoryCache memoryCache)
        {
            _tariffRepository = tariffRepository ?? throw new ArgumentNullException(nameof(tariffRepository));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _tariffs = new List<Tariff>();
        }

        public void AddTariff(Tariff tariff)
        {
            _tariffs.Add(tariff);
        }

        public async Task<IEnumerable<Tariff>> GetTariffsAsync()
        {
            var tariffs = await _tariffRepository.GetAllTariffsAsync();

            return tariffs;
        }

        public async Task<Tariff?> GetTariffByIdAsync(int id)
        {
            return await _tariffRepository.GetTariffByIdAsync(id);
        }

        public async Task AddTariffAsync(Tariff tariff)
        {
            try
            {
                await _tariffRepository.AddTariffAsync(tariff);

                _memoryCache.Remove(TariffsCacheKey);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Failed to add tariff. Duplicate Id or Name.", ex);
            }
        }

        public async Task<IEnumerable<BestTariff>> GetTariffComparisonsAsync(int consumption)
        {
            _tariffs = _tariffRepository.GetAllTariffsAsync().Result.ToList<Tariff>();//(List<Tariff>)

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

        public async Task UpdateTariffAsync(Tariff tariff)
        {
            await _tariffRepository.UpdateTariffAsync(tariff);
            _memoryCache.Remove(TariffsCacheKey);  // Clear the cache when a tariff is updated
        }

        public decimal CalculateAnnualCost(Tariff tariff, int consumption)
        {
            if (tariff.IncludedKwh == null || tariff.AdditionalKwhCost == null)
            {
                throw new InvalidOperationException("Tariff configuration is invalid");//move this to exception
            }

            if (tariff.IncludedKwh == 0)
            {
                var baseCostPerMonth = tariff.BaseCost / 12;
                var annualBaseCost = baseCostPerMonth * 12;
                var annualConsumptionCost = consumption * tariff.AdditionalKwhCost.Value;
                var annualCost = annualBaseCost + annualConsumptionCost;

                return annualCost;
            }
            else
            {
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

        public async Task DeleteTariffAsync(int id)
        {
            await _tariffRepository.DeleteTariffAsync(id);
            _memoryCache.Remove(TariffsCacheKey);  // Clear the cache when a tariff is deleted
        }
    }
}
