using System.IO;
using System.Text.RegularExpressions;

namespace UnityInjector
{
	// This is class is preserved due to some plugins using it through Reflection
	internal static class Extensions
	{
		public static readonly Regex NonAscii = new Regex("[^0-9A-Za-z]");

		public static string UnityInjectorPath { get; set; }

		// Configuration path, but name has to be preserverved for compatibility
		public static string UserDataPath => Path.Combine(UnityInjectorPath, "Config");

		public static string Asciify(this string self)
		{
			return NonAscii.Replace(self, "_");
		}
	}
}