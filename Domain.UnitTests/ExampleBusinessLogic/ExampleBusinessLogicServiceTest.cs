using System.Collections.Generic;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Domain.UnitTests.ExampleBusinessLogic
{
    [TestClass]
    public class ExampleBusinessLogicServiceTest
    {
        private ExampleBusinessLogicService _unit;
        private Mock<IBusinessValueRepository> _repoMock;

        [TestInitialize]
        public void Setup()
        {
            _repoMock = new Mock<IBusinessValueRepository>();
            _unit = new ExampleBusinessLogicService(_repoMock.Object);
        }

        [TestMethod]
        public void EmptyListShouldReturnValueOfZero()
        {
            _repoMock.Setup(m => m.ValuesList).Returns(new List<BusinessValue>());
            var totalValue = _unit.TotalValue();
            Assert.AreEqual(0, totalValue, "");
        }

        
        [TestMethod]
        public void TotalValueShouldBeSumOfAllValues()
        {
            _repoMock.Setup(m => m.ValuesList).Returns(new List<BusinessValue>
            {
                new BusinessValue
                {
                    Customer = "Customer 1",
                    TotalCustomerValue = 1
                },
                new BusinessValue{
                    Customer = "Customer 2",
                    TotalCustomerValue = 2
                }
            });
            var totalValue = _unit.TotalValue();
            Assert.AreEqual(3, totalValue, "");
        }
    }
}
