using System;
using System.Collections.Generic;
using Toolkit.Windows;

namespace ToolKit.Classes
{
    public class RollerText
    {
        public List<string> Text = new();
        public LoopMode LoopMode;
        public DisplayMode DisplayMode;
    }

    //public class RollerWeather
    //{
    //    public bool AutoWeather;
    //    public List<DateTime> Times = new();
    //}

    public class Settings
    {
        public Settings()
        {

        }

        public bool AutoCollapse { get; set; }
        public bool AutoScroll { get; set; }
        public bool Mod_Time { get; set; }
        public Dictionary<TimeOnly, string> Mod_Timer = new();
        //public RollerWeather Mod_Weather = new();
        public RollerText RollerText = new();
    }
}
