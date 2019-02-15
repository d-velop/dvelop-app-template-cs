using System;
using System.Collections.Generic;
using Dvelop.Domain.Repositories;
using Dvelop.Domain.Vacation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Domain.UnitTests.VacationService
{
    [TestClass]
    public class VacationServiceTest
    {
        private Dvelop.Domain.Vacation.VacationService _unit;
        private Mock<IVacationRepository> _vacationRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _vacationRepositoryMock = new Mock<IVacationRepository>();
            _unit = new Dvelop.Domain.Vacation.VacationService(_vacationRepositoryMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_IdIsInvalid_Accept_ShouldThrowArgumentException()
        {
            _unit.AcceptVacation(Guid.NewGuid());
        }

        
        [TestMethod]
        public void Test_IdIsValid_Accept_ShouldReturnTrue()
        {
            var validId = Guid.NewGuid();
            var vacations = new List<VacationModel>
            {
                new VacationModel
                {
                    Id = validId
                }
            };
            _vacationRepositoryMock.SetupGet(x => x.Vacations).Returns(vacations);

            var response = _unit.AcceptVacation(validId);
            Assert.AreEqual(true, response,"Accept did not return true, if called with a valid id");
        }
    }
}