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

        public void AddSound(string name, string file, Utils.SoundType soundType, bool loop = false)
        {
            if (wavSounds.ContainsKey(name) ||oggSounds.ContainsKey(name))
                Trace.WriteLine($"[WARNING] A sound with this name already exists : {name}");
            else
            {
                switch(soundType)
                {
                    case Utils.SoundType.WAV:
                        Utils.Sound<byte> wave = Utils.Sound<byte>.LoadWave(file);
                        wave.SetLooping(loop);
                        wavSounds.Add(name, wave);
                        break;
                    case Utils.SoundType.MP3:
                        Utils.Sound<short> mp3 = Utils.Sound<short>.LoadMP3(file);
                        mp3.SetLooping(loop);
                        mp3Sounds.Add(name, mp3);
                        break;
                    case Utils.SoundType.OGG:
                        Utils.Sound<short> ogg = Utils.Sound<short>.LoadOGG(file);
                        ogg.SetLooping(loop);
                        oggSounds.Add(name, ogg);
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

        public void SetLooping(string name, bool loop)
        {
            if (wavSounds.ContainsKey(name))
                wavSounds[name].SetLooping(loop);
            else if (oggSounds.ContainsKey(name))
                oggSounds[name].SetLooping(loop);
            else if (mp3Sounds.ContainsKey(name))
                mp3Sounds[name].SetLooping(loop);
        }

        public bool GetLooping(string name)
        {
            if (wavSounds.ContainsKey(name))
                return wavSounds[name].GetLooping();
            if (oggSounds.ContainsKey(name))
                return oggSounds[name].GetLooping();
            if (mp3Sounds.ContainsKey(name))
                return mp3Sounds[name].GetLooping();
            return false;
        }

        public Utils.SoundState GetState(string name)
        {
            if (wavSounds.ContainsKey(name))
                return wavSounds[name].GetState();
            if (oggSounds.ContainsKey(name))
                return oggSounds[name].GetState();
            if (mp3Sounds.ContainsKey(name))
                return mp3Sounds[name].GetState();
            return Utils.SoundState.Unknown;
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

        public void PauseSound(string name)
        {
            if (wavSounds.ContainsKey(name))
                wavSounds[name].Pause();
            else if (oggSounds.ContainsKey(name))
                oggSounds[name].Pause();
            else if (mp3Sounds.ContainsKey(name))
                mp3Sounds[name].Pause();
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
