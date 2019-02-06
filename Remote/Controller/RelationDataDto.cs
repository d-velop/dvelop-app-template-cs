namespace Dvelop.Remote.Controller
{
    public class RelationDataDto
    {
        public RelationDataDto(string href, bool templated = false)
        {
            Href = href;
            Templated = templated;
        }
        public string Href { get; set; }

        public bool Templated { get; set; }
    }
}