using System.Reflection;

namespace OzonEdu.MerchandiseService.Platform.Helpers
{
    public static class AssemblyHelper
    {
        public static (string name, string version) GetEntryAssemblyInfo()
        {
            var assembly = Assembly.GetEntryAssembly()?.GetName();
            var name = assembly?.Name ?? "OzonEdu.MerchandiseService";
            var version = assembly?.Version?.ToString() ?? "0.0.0.0";

            return (name, version);
        }
    }
}