namespace Dvelop.Domain.VersionService
{
    public class SemVer
    {
        public int Major {get; set; }
        public int Minor {get; set; }
        public int Patch {get; set; }
        public string Qualifier {get; set; }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{Qualifier}";
        }
    }
}