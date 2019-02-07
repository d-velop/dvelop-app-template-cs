using System;
using System.Collections.Generic;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Domain.Repositories;

namespace Dvelop.Plugins.DynamoDbFake
{
    public class AwsBusinessValueRepository : IBusinessValueRepository
    {

        private List<BusinessValue> _values;
        public AwsBusinessValueRepository()
        {
            _values = new List<BusinessValue>
            {
                new BusinessValue
                {
                    TotalCustomerValue = 10244,
                    Customer = "d.velop Cloud Migration",
                    Id = 0
                },
                new BusinessValue
                {
                    TotalCustomerValue = 754115,
                    Customer = "d.velop AG",
                    Id = 1
                },
                new BusinessValue
                {
                    TotalCustomerValue = 20244436,
                    Customer = "AWS",
                    Id = 2
                }
            };
        }

        public string Implementation => "AWS";

        public List<BusinessValue> ValuesList
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
