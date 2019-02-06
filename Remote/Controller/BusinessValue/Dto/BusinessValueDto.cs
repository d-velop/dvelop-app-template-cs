namespace Dvelop.Remote.Controller.BusinessValue.Dto
{
    public class BusinessValueDto: HalJsonDto
    {
        public int TotalCustomerValue { get; set; }
        public int Id { get; set; }
        public string Customer { get; set; }
    }
}