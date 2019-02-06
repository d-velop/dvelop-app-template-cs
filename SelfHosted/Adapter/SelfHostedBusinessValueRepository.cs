using System;
using System.Collections.Generic;
using Dvelop.Domain.ExampleBusinessLogic;
using Dvelop.Domain.Repositories;

namespace Dvelop.SelfHosted.Adapter
{
    public class SelfHostedBusinessValueRepository : IBusinessValueRepository
    {

        private List<BusinessValue> _values;
        public SelfHostedBusinessValueRepository()
        {
            _values = new List<BusinessValue>
            {
                new BusinessValue
                {
                    Customer= "Windows 2017",
                    Id = 0,
                    TotalCustomerValue = 15422
                },

                new BusinessValue
                {
                    Customer= "Windows Millenium",
                    Id = 1,
                    TotalCustomerValue = 4
                }
            };
        }

        public string Implementation => "SelfHost";

        public List<BusinessValue> ValuesList
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
