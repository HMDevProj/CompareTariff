using API.Controllers;
using Business.Interfaces;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;
using Shared.Models;

namespace Tests
{
    [TestFixture]
    public class TariffsControllerTests
    {
        private Mock<ITariffService> _tariffServiceMock;
        private Mock<ILogger<TariffsController>> _loggerMock;
        private TariffsController _controller;

        [SetUp]
        public void Setup()
        {
            _tariffServiceMock = new Mock<ITariffService>();
            _loggerMock = new Mock<ILogger<TariffsController>>();
            _controller = new TariffsController(_tariffServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllTariffs_ShouldReturnOk_WhenTariffsExist()
        {
            var tariffs = new List<Tariff> { new Tariff { Id = 1, Name = "Product A" } };
            _tariffServiceMock.Setup(s => s.GetTariffsAsync()).ReturnsAsync(tariffs);

            var result = await _controller.GetAllTariffs();

            ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            ClassicAssert.IsNotNull(okResult);
            ClassicAssert.AreEqual(tariffs, okResult.Value);
        }

        [Test]
        public async Task GetTariffById_ShouldReturnOk_WhenTariffExists()
        {
            var tariff = new Tariff { Id = 1, Name = "Product A" };
            _tariffServiceMock.Setup(s => s.GetTariffByIdAsync(tariff.Id)).ReturnsAsync(tariff);

            var result = await _controller.GetTariffById(tariff.Id);

            ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            ClassicAssert.IsNotNull(okResult);
            ClassicAssert.AreEqual(tariff, okResult.Value);
        }

        [Test]
        public async Task GetTariffById_ShouldReturnNotFound_WhenTariffDoesNotExist()
        {
            _tariffServiceMock.Setup(s => s.GetTariffByIdAsync(It.IsAny<int>())).ReturnsAsync((Tariff?)null);

            var result = await _controller.GetTariffById(99);

            ClassicAssert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task AddNewTariff_ShouldReturnCreatedAtAction_WhenTariffIsValid()
        {
            var tariff = new Tariff { Id = 3, Name = "Product C", BaseCost = 150m, IncludedKwh = 1500, AdditionalKwhCost = 0.2m };

            var result = await _controller.AddNewTariff(tariff);

            ClassicAssert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            ClassicAssert.IsNotNull(createdResult);
            ClassicAssert.IsNotNull(createdResult.ActionName);
            ClassicAssert.IsNotNull(createdResult.RouteValues);
            ClassicAssert.IsNotNull(createdResult.Value);

            ClassicAssert.AreEqual(nameof(TariffsController.GetTariffById), createdResult?.ActionName);
            ClassicAssert.AreEqual(tariff.Id, createdResult.RouteValues["id"]);
            ClassicAssert.AreEqual(tariff, createdResult?.Value);
        }

        [Test]
        public async Task UpdateTariff_ShouldReturnOk_WhenTariffIsUpdated()
        {
            var tariff = new Tariff { Id = 1, Name = "Updated Product A", BaseCost = 120m, IncludedKwh = 1100, AdditionalKwhCost = 0.35m };
            _tariffServiceMock.Setup(s => s.GetTariffByIdAsync(tariff.Id)).ReturnsAsync(tariff);

            var result = await _controller.UpdateTariff(tariff.Id, tariff);

            ClassicAssert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            ClassicAssert.AreEqual(tariff, okResult?.Value);
        }

        [Test]
        public void UpdateTariff_ShouldThrowException_WhenTariffIdMismatch()
        {
            var tariff = new Tariff { Id = 1, Name = "Updated Product A" };

            Assert.ThrowsAsync<TariffIdMismatchException>(() => _controller.UpdateTariff(2, tariff));
        }
    }
}
