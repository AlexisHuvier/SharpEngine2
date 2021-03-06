using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Managers
{
    public class ShaderManager
    {
        private Dictionary<string, Graphics.Shader> shaders;

        public ShaderManager()
        {
            shaders = new Dictionary<string, Graphics.Shader>();
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] ShaderManager Initialized");
        }

        public void AddShader(string name, Graphics.Shader shader)
        {
            if (shaders.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A shader with this name already exists : {name}");
            else
            {
                shaders.Add(name, shader);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Shader added : {name}");
            }
        }

        public void RemoveShader(string name)
        {
            if (shaders.ContainsKey(name))
            {
                shaders[name].Unload();
                shaders.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Shader removed : {name}");
            }
            else
                Trace.WriteLine($"[WARNING] A shader with this name doesn't exists : {name}");
        }

        internal Graphics.Shader GetShader(string name)
        {
            if (shaders.ContainsKey(name))
                return shaders[name];
            return null;
        }

        internal void Unload()
        {
            foreach (KeyValuePair<string, Graphics.Shader> shader in shaders)
                shader.Value.Unload();
        }
    }
}
