using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;

namespace BepInEx.UnityInjectorLoader
{
    public static class Hooks
    {
        public static Type GetNamedType(string assemblyName, string typeName)
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x =>
                x.GetName().Name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase));

            return assembly?.GetTypes()
                .FirstOrDefault(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }

        public static void InstallHooks()
        {
            var harmony = HarmonyInstance.Create("org.bepinex.plugins.unityinjectorloader");

            if (HookedMethod == null)
            {
                BepInLogger.Log("[Unity Injector Loader] Unable to find hook!", true, ConsoleColor.Red);
            }
            
            harmony.Patch(
                HookedMethod,
                new HarmonyMethod(typeof(Hooks).GetMethod("LoadSceneHook", BindingFlags.Static | BindingFlags.Public)),
                null);
        }

        public static MethodBase HookedMethod = null;

        public static Action SubscribedAction = null;
        
        public static void LoadSceneHook()
        {
            SubscribedAction?.Invoke();
        }
    }
}
