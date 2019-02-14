using System;

namespace UnityInjector.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class PluginFilterAttribute : Attribute
	{
		public string ExeName { get; }

		public PluginFilterAttribute(string exeName)
		{
			ExeName = exeName;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class PluginNameAttribute : Attribute
	{
		public string Name { get; }

		public PluginNameAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class PluginVersionAttribute : Attribute
	{
		public string Version { get; }

		public PluginVersionAttribute(string name)
		{
			Version = name;
		}
	}
}