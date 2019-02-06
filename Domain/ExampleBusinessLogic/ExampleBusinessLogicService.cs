using System.Collections.Generic;
using System.Linq;
using Dvelop.Domain.Repositories;

namespace Dvelop.Domain.ExampleBusinessLogic
{
    public class ExampleBusinessLogicService: IExampleBusinessLogicService
    {
        private readonly IBusinessValueRepository _businessValueRepository;

        public ExampleBusinessLogicService(IBusinessValueRepository businessValueRepository)
        {
            _businessValueRepository = businessValueRepository;
        }

        public int TotalValue()
        {
            return _businessValueRepository.ValuesList.Select( s=>s.TotalCustomerValue ).Sum();
        }


        // This is just an example of a Service using a repository You should not simple redirect calls from a service to a repository.
        public List<BusinessValue> ValuesList
        {
            get => _businessValueRepository.ValuesList; 
            set => _businessValueRepository.ValuesList = value;
        }
    }
}
