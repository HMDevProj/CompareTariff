using Business.Interfaces;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TariffsController : ControllerBase
    {
        private readonly ITariffService _tariffService;
        private readonly ILogger<TariffsController> _logger;

        public TariffsController(ITariffService tariffService, ILogger<TariffsController> logger)
        {
            _tariffService = tariffService;
            _logger = logger;
        }

        /// <summary>
        /// Get All Tariffs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tariff>>> GetAllTariffs()
        {
            _logger.LogInformation("{MethodName} - Fetching all tariffs", nameof(GetAllTariffs));

            var tariffs = await _tariffService.GetTariffsAsync();

            return Ok(tariffs);
        }

        /// <summary>
        /// Get Tariff by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Tariff>> GetTariffById(int id)
        {
            _logger.LogInformation("{MethodName} - Getting tariff by Id", nameof(GetTariffById));

            var tariff = await _tariffService.GetTariffByIdAsync(id);

            if (tariff == null)
            {
                _logger.LogWarning($"Tariff with id {id} not found");
                return NotFound();
            }

            return Ok(tariff);
        }

        /// <summary>
        /// Get Tariff Comparison in ascending order
        /// </summary>
        /// <param name="consumption"></param>
        /// <returns></returns>
        [HttpGet("consumption/{consumption}")]
        public async Task<IActionResult> GetTariffComparisons(int consumption)
        {
            var results = await _tariffService.GetTariffComparisonsAsync(consumption);

            return Ok(results);
        }

        /// <summary>
        /// Add new Tariff
        /// </summary>
        /// <param name="tariff"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddNewTariff([FromBody] Tariff tariff)
        {
            _logger.LogInformation("{MethodName} - Adding a new Tariff", nameof(AddNewTariff));

            await _tariffService.AddTariffAsync(tariff);

            return CreatedAtAction(nameof(GetTariffById), new { id = tariff.Id }, tariff);
        }

        /// <summary>
        /// Update a Tariff
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tariff"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTariff(int id, [FromBody] Tariff tariff)
        {
            var existingTariff = await _tariffService.GetTariffByIdAsync(id);
            if (existingTariff == null)
            {
                _logger.LogWarning($"Tariff with id {id} not found");
                return NotFound($"Tariff with id {id} not found");
            }

            if (id != tariff.Id)
            {
                throw new TariffIdMismatchException("Tariff ID mismatch.");
            }

            _logger.LogInformation("{MethodName} - Updating tariff with Id {Id}", nameof(UpdateTariff), id);

            try
            {
                await _tariffService.UpdateTariffAsync(tariff);
                var updatedTariff = await _tariffService.GetTariffByIdAsync(id);

                return Ok(updatedTariff);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete a Tariff
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTariff(int id)
        {
            _logger.LogInformation("{MethodName} - Deleting tariff with Id {Id}", nameof(DeleteTariff), id);

            var existingTariff = await _tariffService.GetTariffByIdAsync(id);
            if (existingTariff == null)
            {
                _logger.LogWarning($"Tariff with id {id} not found");
                return NotFound($"Tariff with id {id} not found");
            }

            try
            {
                await _tariffService.DeleteTariffAsync(id);
                return Ok(new { message = "Tariff deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
        }
    }
}
