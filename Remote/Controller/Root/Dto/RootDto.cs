namespace Dvelop.Remote.Controller.Root.Dto
{
    public class RootDto : HalJsonDto
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string Qualifier { get; set; }
    }
}