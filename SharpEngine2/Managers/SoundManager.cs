using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Managers
{
    public class SoundManager
    {
        internal Dictionary<string, Utils.Sound<byte>> wavSounds;
        internal Dictionary<string, Utils.Sound<short>> oggSounds;
        internal Dictionary<string, Utils.Sound<short>> mp3Sounds;

        public SoundManager()
        {
            wavSounds = new Dictionary<string, Utils.Sound<byte>>();
            oggSounds = new Dictionary<string, Utils.Sound<short>>();
            mp3Sounds = new Dictionary<string, Utils.Sound<short>>();
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] SoundManager Initialized");
        }

        public void AddSound(string name, string file, Utils.SoundType soundType)
        {
            if (wavSounds.ContainsKey(name) ||oggSounds.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A sound with this name already exists : {name}");
            else
            {
                switch(soundType)
                {
                    case Utils.SoundType.WAV:
                        wavSounds.Add(name, Utils.Sound<byte>.LoadWave(file));
                        break;
                    case Utils.SoundType.MP3:
                        mp3Sounds.Add(name, Utils.Sound<short>.LoadMP3(file));
                        break;
                    case Utils.SoundType.OGG:
                        oggSounds.Add(name, Utils.Sound<short>.LoadOGG(file));
                        break;
                }
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound added : {name}");
            }
        }

        public void RemoveSound(string name)
        {
            if (wavSounds.ContainsKey(name))
            {
                wavSounds[name].Unload();
                wavSounds.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound removed : {name}");
            }
            else if (oggSounds.ContainsKey(name))
            {
                oggSounds[name].Unload();
                oggSounds.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound removed : {name}");
            }
            else if (mp3Sounds.ContainsKey(name))
            {
                mp3Sounds[name].Unload();
                mp3Sounds.Remove(name);
                Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] Sound removed : {name}");
            }
            else
                Trace.WriteLine($"[WARNING] A sound with this name doesn't exists : {name}");
        }

        public void PlaySound(string name)
        {
            if (wavSounds.ContainsKey(name))
                wavSounds[name].Play();
            else if (oggSounds.ContainsKey(name))
                oggSounds[name].Play();
            else if (mp3Sounds.ContainsKey(name))
                mp3Sounds[name].Play();
        }

        public void StopSound(string name)
        {
            if (wavSounds.ContainsKey(name))
                wavSounds[name].Stop();
            else if (oggSounds.ContainsKey(name))
                oggSounds[name].Stop();
            else if (mp3Sounds.ContainsKey(name))
                mp3Sounds[name].Stop();
        }

        internal void Unload()
        {
            foreach (KeyValuePair<string, Utils.Sound<byte>> sound in wavSounds)
                sound.Value.Unload();
            foreach (KeyValuePair<string, Utils.Sound<short>> sound in oggSounds)
                sound.Value.Unload();
            foreach (KeyValuePair<string, Utils.Sound<short>> sound in mp3Sounds)
                sound.Value.Unload();
        }
    }
}
