using System.Collections.Generic;
using System.Diagnostics;
using SharpFont;

namespace SE2.Managers
{
    public class FontManager
    {
        internal static Library lib = new Library();

        private Dictionary<string, Utils.Font> fonts;

        public FontManager()
        {
            fonts = new Dictionary<string, Utils.Font>();
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] FontManager Initialized");
        }

        public void AddFont(string name, string file, uint size)
        {
            if (fonts.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A font with this name already exists : {name}");
            else
            {
                fonts.Add(name, new Utils.Font(file, size));
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Font added : {name}");
            }
        }

        public void RemoveFont(string name)
        {
            if (fonts.ContainsKey(name))
            {
                fonts[name].Unload();
                fonts.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Font removed : {name}");
            }
            else
                Trace.WriteLine($"[WARNING] A font with this name doesn't exists : {name}");
        }

        internal Utils.Font GetFont(string name)
        {
            if (fonts.ContainsKey(name))
                return fonts[name];
            else
                Trace.WriteLine($"[WARNING] A font with this name doesn't exists : {name}");
            return null;
        }
        internal void Unload()
        {
            foreach (KeyValuePair<string, Utils.Font> font in fonts)
                font.Value.Unload();
        }
    }
}
