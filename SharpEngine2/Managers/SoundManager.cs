using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Managers
{
    public class SoundManager
    {
        internal Dictionary<string, Utils.Sound<byte>> soundsBytes;
        internal Dictionary<string, Utils.Sound<float>> soundsFloat;

        public SoundManager()
        {
            soundsBytes = new Dictionary<string, Utils.Sound<byte>>();
            soundsFloat = new Dictionary<string, Utils.Sound<float>>();
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] SoundManager Initialized");
        }

        public void AddSound(string name, string file, Utils.SoundType soundType)
        {
            if (soundsBytes.ContainsKey(name) ||soundsFloat.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A sound with this name already exists : {name}");
            else
            {
                switch(soundType)
                {
                    case Utils.SoundType.WAV:
                        soundsBytes.Add(name, Utils.Sound<byte>.LoadWave(file));
                        break;
                    case Utils.SoundType.MP3:
                        soundsBytes.Add(name, Utils.Sound<byte>.LoadMP3(file));
                        break;
                    case Utils.SoundType.OGG:
                        soundsFloat.Add(name, Utils.Sound<float>.LoadOGG(file));
                        break;
                }
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound added : {name}");
            }
        }

        public void RemoveSound(string name)
        {
            if (soundsBytes.ContainsKey(name))
            {
                soundsBytes[name].Unload();
                soundsBytes.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound removed : {name}");
            }
            else if (soundsFloat.ContainsKey(name))
            {
                soundsFloat[name].Unload();
                soundsFloat.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound removed : {name}");
            }
            else
                Trace.WriteLine($"[WARNING] A sound with this name doesn't exists : {name}");
        }

        public void PlaySound(string name)
        {
            if (soundsBytes.ContainsKey(name))
                soundsBytes[name].Play();
            else if (soundsFloat.ContainsKey(name))
                soundsFloat[name].Play();
            else
                Trace.WriteLine($"[WARNING] A sound with this name doesn't exists : {name}");
        }

        public void StopSound(string name)
        {
            if (soundsBytes.ContainsKey(name))
                soundsBytes[name].Stop();
            else if (soundsFloat.ContainsKey(name))
                soundsFloat[name].Stop();
            else
                Trace.WriteLine($"[WARNING] A sound with this name doesn't exists : {name}");
        }

        internal void Unload()
        {
            foreach (KeyValuePair<string, Utils.Sound<byte>> sound in soundsBytes)
                sound.Value.Unload();
            foreach (KeyValuePair<string, Utils.Sound<float>> sound in soundsFloat)
                sound.Value.Unload();
        }
    }
}
