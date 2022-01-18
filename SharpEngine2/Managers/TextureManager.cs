using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Managers
{
    public class TextureManager
    {
        private Dictionary<string, Utils.Texture> textures;

        public TextureManager()
        {
            textures = new Dictionary<string, Utils.Texture>();
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] TextureManager Initialized");
        }

        public void AddTexture(string name, string file, Utils.TextureFilter minFilter = Utils.TextureFilter.Linear, Utils.TextureFilter magFilter = Utils.TextureFilter.Linear)
        {
            if (textures.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A texture with this name already exists : {name}");
            else
            {
                textures.Add(name, Utils.Texture.LoadFromFile(file, minFilter, magFilter));
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Texture added : {name}");
            }
        }

        public void RemoveTexture(string name)
        {
            if (textures.ContainsKey(name))
            {
                textures[name].Unload();
                textures.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Texture removed : {name}");
            }
            else
                Trace.WriteLine($"[WARNING] A texture with this name doesn't exists : {name}");
        }

        internal Utils.Texture GetTexture(string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];
            else
                Trace.WriteLine($"[WARNING] A texture with this name doesn't exists : {name}");
            return null;
        }
        internal void Unload()
        {
            foreach (KeyValuePair<string, Utils.Texture> texture in textures)
                texture.Value.Unload();
        }
    }
}
