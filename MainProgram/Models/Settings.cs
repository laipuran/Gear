using ProngedGear.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProngedGear.Models
{
    public class RollerText
    {
        public List<string> Text = new();
        public LoopMode LoopMode;
        public DisplayMode DisplayMode;
    }

    public class Settings
    {
        public Settings()
        {

        }

        #region Classifier Settings
        public Dictionary<string, string> Subjects { get; set; } = new();
        #endregion

        #region Notifier Settings
        public bool AutoCollapse { get; set; }
        public bool AutoScroll { get; set; }
        public bool Mod_Time { get; set; }
        public Dictionary<TimeOnly, string> Mod_Timer { get; set; } = new();
        public RollerText RollerText { get; set; } = new();
        #endregion  
    }
}
