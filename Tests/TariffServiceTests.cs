using Business.Services;
using Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework.Legacy;
using Shared.Models;

namespace Tests
{
    public class TariffServiceTests
    {
        private Mock<ITariffRepository> _tariffRepositoryMock;
        private TariffService _tariffService;
        private IMemoryCache _mockCache;

        [SetUp]
        public void Setup()
        {
            _tariffRepositoryMock = new Mock<ITariffRepository>();
            _mockCache = new MemoryCache(new MemoryCacheOptions());
            _tariffService = new TariffService(_tariffRepositoryMock.Object, _mockCache);
        }

        [TearDown]
        public void TearDown()
        {
            _mockCache.Dispose();
        }
        [Test]
        public async Task AddTariffAsync_ShouldAddTariff_WhenTariffIsValid()
        {
            var tariff = new Tariff { Id = 3, Name = "Product C", BaseCost = 150m, IncludedKwh = 1500, AdditionalKwhCost = 0.2m };

            await _tariffService.AddTariffAsync(tariff);

            _tariffRepositoryMock.Verify(r => r.AddTariffAsync(It.IsAny<Tariff>()), Times.Once);
        }

        [Test]
        public async Task GetTariffByIdAsync_ShouldReturnTariff_WhenTariffExists()
        {
            var tariff = new Tariff { Id = 1, Name = "Product A", BaseCost = 100m, IncludedKwh = 1000, AdditionalKwhCost = 0.3m };
            _tariffRepositoryMock.Setup(r => r.GetTariffByIdAsync(tariff.Id)).ReturnsAsync(tariff);

            var result = await _tariffService.GetTariffByIdAsync(tariff.Id);

            ClassicAssert.AreEqual(tariff, result);
        }

        [Test]
        public async Task UpdateTariffAsync_ShouldUpdateTariff_WhenTariffExists()
        {
            var tariff = new Tariff { Id = 1, Name = "Updated Product A", BaseCost = 120m, IncludedKwh = 1100, AdditionalKwhCost = 0.35m };
            _tariffRepositoryMock.Setup(r => r.GetTariffByIdAsync(tariff.Id)).ReturnsAsync(tariff);

            await _tariffService.UpdateTariffAsync(tariff);

            _tariffRepositoryMock.Verify(r => r.UpdateTariffAsync(tariff), Times.Once);
        }

        [Test]
        public async Task DeleteTariffAsync_ShouldDeleteTariff_WhenTariffExists()
        {
            var tariff = new Tariff { Id = 1, Name = "Product A", BaseCost = 100m, IncludedKwh = 1000, AdditionalKwhCost = 0.3m };
            _tariffRepositoryMock.Setup(r => r.GetTariffByIdAsync(tariff.Id)).ReturnsAsync(tariff);

            await _tariffService.DeleteTariffAsync(tariff.Id);

            _tariffRepositoryMock.Verify(r => r.DeleteTariffAsync(tariff.Id), Times.Once);
        }

        [Test]
        public async Task GetTariffsAsync_ReturnsTariffs()
        {
            // Arrange
            var tariffs = new List<Tariff>
            {
                new Tariff { Id = 1, Name = "Product A", Type = 1, BaseCost = 5, AdditionalKwhCost = 22 },
                new Tariff { Id = 2, Name = "Product B", Type = 2, IncludedKwh = 4000, BaseCost = 800, AdditionalKwhCost = 30 }
            };

            _tariffRepositoryMock.Setup(repo => repo.GetAllTariffsAsync()).ReturnsAsync(tariffs);

            // Act
            var result = await _tariffService.GetTariffsAsync();

            // Assert
            ClassicAssert.AreEqual(2, result.Count());
            ClassicAssert.AreEqual("Product A", result.First().Name);
        }

        [Test]
        public async Task GetTariffByIdAsync_ReturnsTariff()
        {
            // Arrange
            var tariff = new Tariff { Id = 1, Name = "Product A", Type = 1, BaseCost = 5, AdditionalKwhCost = 22 };
            _tariffRepositoryMock.Setup(repo => repo.GetTariffByIdAsync(1)).ReturnsAsync(tariff);

            // Act
            var result = await _tariffService.GetTariffByIdAsync(1);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Product A", result.Name);
        }

        [Test]
        public async Task AddTariffAsync_AddsTariff()
        {
            // Arrange
            var tariff = new Tariff { Id = 1, Name = "Product A", Type = 1, BaseCost = 5, AdditionalKwhCost = 22 };

            // Act
            await _tariffService.AddTariffAsync(tariff);

            // Assert
            _tariffRepositoryMock.Verify(repo => repo.AddTariffAsync(tariff), Times.Once);
        }

        [Test]
        public async Task GetTariffComparisons_ShouldReturnTariffsInAscendingOrderOfCost()
        {
            // Arrange
            int consumption = 4500;

            // Act
            var results = await _tariffService.GetTariffComparisonsAsync(consumption);

            // Assert
            Assert.That(results, Is.Ordered.By("AnnualCost"));
        }
    }
}
