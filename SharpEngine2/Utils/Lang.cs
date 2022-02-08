using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;

namespace SE2.Utils
{
    public class Lang
    {
        private Dictionary<string, string> translations;
        private string _file;
        public string file
        {
            get => _file;
            set
            {
                _file = value;
                translations.Clear();
                if (File.Exists(value))
                {
                    foreach (string line in File.ReadAllLines(value))
                    {
                        if (line.Split(": ").Length == 2)
                            translations.Add(line.Split(": ")[0], line.Split(": ")[1].Replace("\n", ""));
                        else
                            Trace.WriteLine($"[ERROR] Unknown format of Lang : {line} (Use : 'key: value')");
                    }
                }
                else
                    Trace.WriteLine($"[ERROR] Lang file doesn't exist : {value}");
            }
        }

        public Lang(string file)
        {
            translations = new Dictionary<string, string>();
            this.file = file;
        }

        public string GetTranslation(string key, object[] objects) => translations.ContainsKey(key) ? String.Format(translations[key], objects) : $"Unknown translation : {key}";
        public string GetTranslation(string key) => translations.ContainsKey(key) ? translations[key] : $"Unknown translation : {key}";
    }
}
