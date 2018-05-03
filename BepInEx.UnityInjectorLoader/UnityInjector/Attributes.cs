using System;

namespace UnityInjector.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginFilterAttribute : Attribute
    {
        public PluginFilterAttribute(string exeName)
        {
            ExeName = exeName;
        }

        public string ExeName { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginNameAttribute : Attribute
    {
        public PluginNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginVersionAttribute : Attribute
    {
        public PluginVersionAttribute(string name)
        {
            Version = name;
        }

        public string Version { get; }
    }
}