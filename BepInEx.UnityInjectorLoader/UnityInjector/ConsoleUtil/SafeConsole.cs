using System;
using BepInEx.UnityInjectorLoader;

namespace UnityInjector.ConsoleUtil
{
    public static class SafeConsole
    {
        public static ConsoleColor BackgroundColor
        {
            set => ConsoleHelper.SetBackgroundColor(value);
            get => ConsoleHelper.GetBackgroundColor();
        }

        public static ConsoleColor ForegroundColor
        {
            set => ConsoleHelper.SetForegroundColor(value);
            get => ConsoleHelper.GetForegroundColor();
        }
    }
}