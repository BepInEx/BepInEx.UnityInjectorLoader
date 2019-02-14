using System;
using System.Reflection;

namespace BepInEx.UnityInjectorLoader
{
	internal static class ConsoleHelper
	{
		public static readonly GetColor GetBackgroundColor;
		public static readonly GetColor GetForegroundColor;
		public static readonly SetColor SetBackgroundColor;
		public static readonly SetColor SetForegroundColor;

		public delegate ConsoleColor GetColor();

		public delegate void SetColor(ConsoleColor color);

		static ConsoleHelper()
		{
			var bepinSafeConsole = typeof(BaseUnityPlugin).Assembly.GetType("BepInEx.ConsoleUtil.Kon", false, false);

			var bgColorProperty =
				bepinSafeConsole.GetProperty("BackgroundColor", BindingFlags.Static | BindingFlags.Public);
			var fgColorProperty =
				bepinSafeConsole.GetProperty("ForegroundColor", BindingFlags.Static | BindingFlags.Public);

			SetBackgroundColor = (SetColor)Delegate.CreateDelegate(typeof(SetColor), bgColorProperty.GetSetMethod());
			SetForegroundColor = (SetColor)Delegate.CreateDelegate(typeof(SetColor), fgColorProperty.GetSetMethod());

			GetForegroundColor = (GetColor)Delegate.CreateDelegate(typeof(GetColor), fgColorProperty.GetGetMethod());
			GetBackgroundColor = (GetColor)Delegate.CreateDelegate(typeof(GetColor), bgColorProperty.GetGetMethod());
		}
	}
}