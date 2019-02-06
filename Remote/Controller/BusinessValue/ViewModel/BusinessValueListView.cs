using System.Collections.Generic;

namespace Dvelop.Remote.Controller.BusinessValue.ViewModel
{
    public class BusinessValueListView
    {
        public List<BusinessValueListViewEntry> Values = new List<BusinessValueListViewEntry>();
        public string Caption { get; set; }
        public int Total { get; set; }
    }
}