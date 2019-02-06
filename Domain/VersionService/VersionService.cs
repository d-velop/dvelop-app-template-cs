using System.Reflection;
using System.Text.RegularExpressions;

namespace Dvelop.Domain.VersionService
{
    public class VersionService : IVersionService
    {
        public SemVer Version
        {
            get
            {
                var assemblyInfoVersion = typeof(VersionService).Assembly.GetCustomAttribute(typeof (AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
                var version = Regex.Match(assemblyInfoVersion?.InformationalVersion??"", @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)[\s-]*(?<qualifier>.*)");
                var qualifier = version.Groups["qualifier"].Value;
                if (string.IsNullOrWhiteSpace(qualifier))
                {
                    qualifier = "0";
                }
                return new SemVer
                {
                    Major = int.Parse(version.Groups["major"].Value),
                    Minor = int.Parse(version.Groups["minor"].Value),
                    Patch = int.Parse(version.Groups["patch"].Value),
                    Qualifier = qualifier
                };

            }
        }
    }
}
