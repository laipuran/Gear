using System;
using System.Collections.Generic;
using Toolkit.Windows;

namespace ToolKit.Classes
{
    public struct RollerText
    {
        public string? Text;
        public LoopMode Mode;
    }

    public struct RollerWeather
    {
        public bool AutoWeather;
        public List<DateTime> Times;
    }

    public class Settings
    {
        public Settings()
        {

        }

        public bool AutoCollapse { get; set; }
        public bool AutoScroll { get; set; }
        public bool Mod_Time { get; set; }
        public RollerWeather Mod_Weather { get; set; }
        public RollerText RollerText { get; set; }
    }
}
