using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace BepInEx.UnityInjectorLoader
{
    [BepInPlugin("org.bepinex.plugins.unityinjectorloader", "UnityInjector Plugin Loader", "1.0")]
    public class UnityInjectorLoader : BaseUnityPlugin
    {
        private GameObject managerObject;
        
        public string AssemblyName => this.GetEntry("Entrypoint-AssemblyName", "Assembly-CSharp");
        public string TypeName => this.GetEntry("Entrypoint-TypeName", "SceneLogo");
        public string MethodName => this.GetEntry("Entrypoint-MethodName", "Start");

        public UnityInjectorLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveUnityInjector;

            Hooks.SubscribedAction = Init;

            Hooks.HookedMethod = Hooks.GetNamedType(AssemblyName, TypeName)?.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            Hooks.InstallHooks();
        }

        public void Init()
        {
            DontDestroyOnLoad(this);

            if (!this.HasEntry("unityInjectorLocation"))
            {
                this.SetEntry("unityInjectorLocation", "UnityInjector");
                Config.SaveConfig();
            }

            Extensions.UnityInjectorPath = this.GetEntry("unityInjectorLocation");

            if (!Directory.Exists(Extensions.UnityInjectorPath))
            {
                Logger.Log(LogLevel.Info, $"No UnityInjector path found in {Extensions.UnityInjectorPath}. Creating one...");
                try
                {
                    Directory.CreateDirectory(Extensions.UnityInjectorPath);
                    Directory.CreateDirectory(Extensions.UserDataPath);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Fatal, $"Failed to create UnityInjector folder! Error message: {e.Message}");
                }

                Destroy(this);
                return;
            }

            managerObject = new GameObject("UnityInjector");

            Logger.Log(LogLevel.Info, "UnityInjector started");

            string currentProcess = Process.GetCurrentProcess().ProcessName;

            List<Type> plugins = new List<Type>();

            foreach (string pluginDll in Directory.GetFiles(Extensions.UnityInjectorPath, "*.dll"))
            {
                try
                {
                    Assembly pluginAssembly = Assembly.LoadFile(pluginDll);
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        if (type.IsAbstract || type.IsInterface || !typeof(PluginBase).IsAssignableFrom(type))
                            continue;

                        PluginFilterAttribute[] filterAttributes =
                                (PluginFilterAttribute[])type.GetCustomAttributes(typeof(PluginFilterAttribute), false);

                        if (filterAttributes.Length == 0
                            || filterAttributes.Select(attr => attr.ExeName)
                                               .Contains(currentProcess, StringComparer.InvariantCultureIgnoreCase))
                        {
                            plugins.Add(type);

                            PluginNameAttribute name =
                                    type.GetCustomAttributes(typeof(PluginNameAttribute), false).FirstOrDefault() as
                                            PluginNameAttribute;

                            PluginVersionAttribute version =
                                    type.GetCustomAttributes(typeof(PluginVersionAttribute), false).FirstOrDefault() as
                                            PluginVersionAttribute;

                            Logger.Log(LogLevel.Info, $"UnityInjector: loaded {name?.Name ?? pluginAssembly.GetName().Name} {version?.Version ?? pluginAssembly.GetName().Version.ToString()}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, $"Failed to load {pluginDll}. Stack trace:\n{e}");
                }
            }

            if (plugins.Count == 0)
            {
                Logger.Log(LogLevel.Info, "UnityInjector: No plugins found!");
                Destroy(managerObject);
                Destroy(this);
                return;
            }

            foreach (Type plugin in plugins)
                try
                {
                    managerObject.AddComponent(plugin);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, $"UnityInjector: Failed to initialize {plugin.Assembly.GetName().Name}\nError: {e}");
                }

            Logger.Log(LogLevel.Info, "UnityInjector: All plugins loaded");

            managerObject.SetActive(true);
        }

        private static Assembly ResolveUnityInjector(object sender, ResolveEventArgs args)
        {
            // If some assembly requests UnityInjector assembly, serve them with this one
            // This assembly contains a minimal UnityInjector wrapper, that allows plug-ins to work (even if they do some reflection magic)
            // Harr harr harr...

            string name = new AssemblyName(args.Name).Name.ToLower();

            if (name != "unityinjector" && name != "exini")
                return null;

            return Assembly.GetExecutingAssembly();
        }
    }
}