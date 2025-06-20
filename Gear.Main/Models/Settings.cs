﻿using Gear.Windows;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gear.Models
{

    public class Subject
    {
        public enum SchoolSubject
        {
            Chinese,
            Math,
            English,
            Physics,
            Chemistry,
            Biology,
            Politics,
            History,
            Geography
        }

        public static SchoolSubject? GetSubjects(string subject)
        {
            var dict = new Dictionary<string, SchoolSubject>
            {
                { "Chinese", SchoolSubject.Chinese },
                { "Math", SchoolSubject.Math },
                { "English", SchoolSubject.English },
                { "Physics", SchoolSubject.Physics },
                { "Chemistry", SchoolSubject.Chemistry },
                { "Biology", SchoolSubject.Biology },
                { "Politics", SchoolSubject.Politics },
                { "History", SchoolSubject.History },
                { "Geography", SchoolSubject.Geography }
            };

            dict.TryGetValue(subject, out SchoolSubject subjects);
            return subjects;
        }
    }

    public class RollerText
    {
        public List<string> Text = new();
        public LoopMode LoopMode;
        public DisplayMode DisplayMode;
    }

    public class Settings
    {
        static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Pronged Gear\\Settings.json";
        public Settings()
        {
            Subjects = new Subject.SchoolSubject[6] {
                Subject.SchoolSubject.Chinese,
                Subject.SchoolSubject.Math,
                Subject.SchoolSubject.English,
                Subject.SchoolSubject.Physics,
                Subject.SchoolSubject.Chemistry,
                Subject.SchoolSubject.Biology
            };
        }

        public bool AutoStart { get; set; }

        #region Classifier Settings
        public Subject.SchoolSubject[] Subjects { get; set; } = new Subject.SchoolSubject[6];
        public string RootDirectory { get; set; } = "D:\\Subjects";
        #endregion

        #region Notifier Settings
        public bool AutoCollapse { get; set; }
        public bool AutoScroll { get; set; }
        public bool Mod_Time { get; set; }
        public Dictionary<TimeOnly, string> Mod_Timing { get; set; } = new();
        public RollerText RollerText { get; set; } = new();
        public bool EnableCountdown { get; set; } = new();
        public string? CountDownEventName { get; set; }
        public DateTime? CountdownDate { get; set; } = new();
        #endregion

        public static Settings GetSettings()
        {
            if (!File.Exists(path))
                return new();
            var json = File.ReadAllText(path);

            Settings? settings = JsonConvert.DeserializeObject<Settings>(json);
            if (settings is null)
                return new();

            return settings;
        }

        public static void Save(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            var directory = Directory.GetParent(path);

            if (directory is null) return;
            Directory.CreateDirectory(directory.FullName);
            File.WriteAllText(path, json);
        }
    }
}
