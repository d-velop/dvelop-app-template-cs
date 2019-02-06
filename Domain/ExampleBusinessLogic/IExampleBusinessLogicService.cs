using System.Collections.Generic;

namespace Dvelop.Domain.ExampleBusinessLogic
{
    public interface IExampleBusinessLogicService
    {
        /// <summary>
        /// A list of Customer
        /// </summary>
        /// <returns></returns>
        List<BusinessValue> ValuesList { get; set; }
        
        /// <summary>
        /// The total Value
        /// </summary>
        /// <returns>TotalValue</returns>
        int TotalValue();

    }
}