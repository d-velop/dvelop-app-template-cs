namespace Dvelop.Domain.VersionService
{
    public interface IVersionService
    {
        SemVer Version { get; }        
    }
}