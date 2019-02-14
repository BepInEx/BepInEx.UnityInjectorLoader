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
		internal new static ManualLogSource Logger;
		private GameObject managerObject;

		public string AssemblyName => this.GetEntry("Entrypoint-AssemblyName", "Assembly-CSharp");
		public string TypeName => this.GetEntry("Entrypoint-TypeName", "SceneLogo");
		public string MethodName => this.GetEntry("Entrypoint-MethodName", "Start");

		public UnityInjectorLoader()
		{
			Logger = base.Logger;

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
				Logger.LogInfo($"No UnityInjector path found in {Extensions.UnityInjectorPath}. Creating one...");
				try
				{
					Directory.CreateDirectory(Extensions.UnityInjectorPath);
					Directory.CreateDirectory(Extensions.UserDataPath);
				}
				catch (Exception e)
				{
					Logger.LogFatal($"Failed to create UnityInjector folder! Error message: {e.Message}");
				}

				Destroy(this);
				return;
			}

			managerObject = new GameObject("UnityInjector");

			Logger.LogInfo("UnityInjector started");

			string currentProcess = Process.GetCurrentProcess().ProcessName;

			var plugins = new List<Type>();

			foreach (string pluginDll in Directory.GetFiles(Extensions.UnityInjectorPath, "*.dll"))
				try
				{
					var pluginAssembly = Assembly.LoadFile(pluginDll);
					foreach (var type in pluginAssembly.GetTypes())
					{
						if (type.IsAbstract || type.IsInterface || !typeof(PluginBase).IsAssignableFrom(type))
							continue;

						var filterAttributes =
							(PluginFilterAttribute[])type.GetCustomAttributes(typeof(PluginFilterAttribute), false);

						if (filterAttributes.Length == 0
							|| filterAttributes.Select(attr => attr.ExeName)
											   .Contains(currentProcess, StringComparer.InvariantCultureIgnoreCase))
						{
							plugins.Add(type);

							var name =
								type.GetCustomAttributes(typeof(PluginNameAttribute), false).FirstOrDefault() as
									PluginNameAttribute;

							var version =
								type.GetCustomAttributes(typeof(PluginVersionAttribute), false).FirstOrDefault() as
									PluginVersionAttribute;

							Logger.LogInfo($"UnityInjector: loaded {name?.Name ?? pluginAssembly.GetName().Name} {version?.Version ?? pluginAssembly.GetName().Version.ToString()}");
						}
					}
				}
				catch (Exception e)
				{
					Logger.LogError($"Failed to load {pluginDll}. Stack trace:\n{e}");
				}

			if (plugins.Count == 0)
			{
				Logger.LogInfo("UnityInjector: No plugins found!");
				Destroy(managerObject);
				Destroy(this);
				return;
			}

			foreach (var plugin in plugins)
				try
				{
					managerObject.AddComponent(plugin);
				}
				catch (Exception e)
				{
					Logger.LogError($"UnityInjector: Failed to initialize {plugin.Assembly.GetName().Name}\nError: {e}");
				}

			Logger.LogInfo("UnityInjector: All plugins loaded");

			managerObject.SetActive(true);
		}

		private static Assembly ResolveUnityInjector(object sender, ResolveEventArgs args)
		{
			// If some assembly requests UnityInjector assembly, serve them with this one
			// This assembly contains a minimal UnityInjector wrapper, that allows plug-ins to work (even if they do some reflection magic)
			// Harr harr harr...

			string name = new AssemblyName(args.Name).Name.ToLower();
			return name != "unityinjector" ? null : Assembly.GetExecutingAssembly();
		}
	}
}