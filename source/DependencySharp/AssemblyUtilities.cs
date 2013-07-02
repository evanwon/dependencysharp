using System.IO;
using System.Reflection;

namespace DependencySharp
{
    /// <summary>
    /// Various assembly utilities.
    /// </summary>
    public static class AssemblyUtilities
    {
        /// <summary>
        /// Returns the path on disk of the executing assembly.
        /// </summary>
        public static string ExecutingAssemblyPath
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var assemblyDirectory = Path.GetDirectoryName(assembly.Location);

                return assemblyDirectory + "\\";
            }
        }
    }
}
