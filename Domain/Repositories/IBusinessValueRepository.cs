using System.Collections.Generic;
using Dvelop.Domain.ExampleBusinessLogic;

namespace Dvelop.Domain.Repositories
{
    public interface IBusinessValueRepository
    {
        string Implementation { get; }
        List<BusinessValue> ValuesList { get; set; }
    }
}