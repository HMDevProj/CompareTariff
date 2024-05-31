using Data.Contexts;
using Data.Interfaces;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Data.Repositories
{
    public class TariffRepository : ITariffRepository
    {
        private readonly TariffContext _context;

        public TariffRepository(TariffContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tariff>> GetAllTariffsAsync()
        {
            return await _context.Tariffs.ToListAsync();
        }

        public async Task AddTariffAsync(Tariff tariff)
        {
            var existingTariff = await GetTariffByIdAsync(tariff.Id);
            if (existingTariff != null)
            {
                throw new DuplicateTariffException("A tariff with the same Id already exists.");
            }

            var existingTariffByName = await _context.Tariffs
                .FirstOrDefaultAsync(t => t.Name == tariff.Name);
            if (existingTariffByName != null)
            {
                throw new DuplicateTariffException("A tariff with the same Name already exists.");
            }

            _context.Tariffs.Add(tariff);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") ?? false)
            {
                throw new DuplicateTariffException("A tariff with the same Id or Name already exists.");
            }
        }

        public async Task<Tariff?> GetTariffByIdAsync(int id)
        {
            return await _context.Tariffs.FindAsync(id);
        }

        public async Task UpdateTariffAsync(Tariff tariff)
        {
            var existingTariff = await GetTariffByIdAsync(tariff.Id);
            if (existingTariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            _context.Entry(existingTariff).CurrentValues.SetValues(tariff);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTariffAsync(int id)
        {
            var tariff = await GetTariffByIdAsync(id);
            if (tariff == null)
            {
                throw new KeyNotFoundException("Tariff not found.");
            }

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();
        }
    }
}
