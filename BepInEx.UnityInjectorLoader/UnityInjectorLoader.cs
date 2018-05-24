using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace BepInEx.UnityInjectorLoader
{
    [BepInPlugin("org.bepinex.plugins.unityinjectorloader", "UnityInjector Plugin Loader", "1.0")]
    public class UnityInjectorLoader : BaseUnityPlugin
    {
        private GameObject managerObject;

        public UnityInjectorLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveUnityInjector;
        }

        public void Start()
        {
            DontDestroyOnLoad(this);
            Application.logMessageReceived += OnLogReceived;

            if (!this.HasEntry("unityInjectorLocation"))
            {
                this.SetEntry("unityInjectorLocation", "UnityInjector");
                Config.SaveConfig();
            }

            Extensions.UnityInjectorPath = this.GetEntry("unityInjectorLocation");

            if (!Directory.Exists(Extensions.UnityInjectorPath))
            {
                BepInLogger.Log($"No UnityInjector path found in {Extensions.UnityInjectorPath}. Creating one...");
                try
                {
                    Directory.CreateDirectory(Extensions.UnityInjectorPath);
                    Directory.CreateDirectory(Extensions.UserDataPath);
                }
                catch (Exception e)
                {
                    BepInLogger.Log($"Failed to create UnityInjector folder! Error message: {e.Message}",
                                    false,
                                    ConsoleColor.Yellow);
                }

                Destroy(this);
                return;
            }

            managerObject = new GameObject("UnityInjector");

            BepInLogger.Log("UnityInjector started");

            string currentProcess = Process.GetCurrentProcess().ProcessName;

            List<Type> plugins = new List<Type>();

            foreach (string pluginDll in Directory.GetFiles(Extensions.UnityInjectorPath, "*.dll"))
            {
                Assembly pluginAssembly;
                try
                {
                    pluginAssembly = Assembly.LoadFile(pluginDll);
                }
                catch (Exception e)
                {
                    BepInLogger.Log($"Failed to load {pluginDll}. Stack trace:\n{e}", false, ConsoleColor.Red);
                    continue;
                }

                foreach (Type type in pluginAssembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || !typeof(PluginBase).IsAssignableFrom(type))
                        continue;

                    PluginFilterAttribute[] filterAttributes =
                            (PluginFilterAttribute[]) type.GetCustomAttributes(typeof(PluginFilterAttribute), false);

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

                        BepInLogger
                                .Log($"UnityInjector: loaded {name?.Name ?? pluginAssembly.GetName().Name} {version?.Version ?? pluginAssembly.GetName().Version.ToString()}");
                    }
                }
            }

            if (plugins.Count == 0)
            {
                BepInLogger.Log("UnityInjector: No plugins found!");
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
                    BepInLogger.Log($"UnityInjector: Failed to initialize {plugin.Assembly.GetName().Name}\nError: {e}",
                                    false,
                                    ConsoleColor.Red);
                }

            BepInLogger.Log("UnityInjector: All plugins loaded");

            managerObject.SetActive(true);
        }

        private static Assembly ResolveUnityInjector(object sender, ResolveEventArgs args)
        {
            // If some assembly requests UnityInjector assembly, serve them with this one
            // This assembly contains a minimal UnityInjector wrapper, that allows plug-ins to work (even if they do some reflection magic)
            // Harr harr harr...

            string name = new AssemblyName(args.Name).Name.ToLower();

            Console.WriteLine($"Getting {name}!");

            if (name != "unityinjector" && name != "exini")
                return null;

            return Assembly.GetExecutingAssembly();
        }

        private static void OnLogReceived(string condition, string stackTrace, LogType type)
        {
            bool showStackTrace = false;
            ConsoleColor color;
            switch (type)
            {
                case LogType.Error:
                    color = ConsoleColor.DarkRed;
                    break;
                case LogType.Assert:
                    color = ConsoleColor.Cyan;
                    break;
                case LogType.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case LogType.Exception:
                    color = ConsoleColor.Red;
                    showStackTrace = true;
                    break;
                case LogType.Log:
                default:
                    color = ConsoleColor.Gray;
                    break;
            }

            BepInLogger.Log(condition, false, color);
            if (showStackTrace)
                BepInLogger.Log($"Stack Trace:\n{stackTrace}", false, color);

            // We have to reset the colors, since BepInEx does not do that for us, and most
            // UnityInjector plug-ins don't do that either
            ConsoleHelper.SetForegroundColor(ConsoleColor.Gray);
            ConsoleHelper.SetBackgroundColor(ConsoleColor.Black);
        }
    }
}