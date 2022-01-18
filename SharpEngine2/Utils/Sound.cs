using OpenTK.Audio.OpenAL;
using System;
using System.IO;
using NLayer;
using NVorbis;
using System.Collections.Generic;

namespace SE2.Utils
{
    public enum SoundType
    {
        OGG,
        WAV,
        MP3
    }
    internal class Sound<T> where T: unmanaged
    {
        public T[] data;
        public int channels;
        public int bitsPerSample;
        public int sampleRate;
        public ALFormat format;
        public int buffer;
        public int source;

        public Sound(T[] d, int c, int bPS, int sR)
        {
            data = d;
            channels = c;
            bitsPerSample = bPS;
            sampleRate = sR;
            format = GetSoundFormat(channels, bitsPerSample);

            buffer = AL.GenBuffer();
            source = AL.GenSource();

            AL.BufferData(buffer, format, data, sampleRate);
            AL.Source(source, ALSourcei.Buffer, buffer);
        }

        public void Play()
        {
            AL.SourcePlay(source);
        }

        public void Stop()
        {
            AL.SourceStop(source);
        }

        public void Unload()
        {
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
        }

        private ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : (bits == 16 ? ALFormat.Mono16 : ALFormat.MonoFloat32Ext);
                case 2: return bits == 8 ? ALFormat.Stereo8 : (bits == 16 ? ALFormat.Stereo16 : ALFormat.StereoFloat32Ext);
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        internal static Sound<float> LoadOGG(string file)
        {
            VorbisReader reader = new VorbisReader(file);

            int num_channels = reader.Channels;
            int bits_per_sample = 32;
            int sample_rate = reader.SampleRate;

            List<float> data = new List<float>();
            float[] buff = new float[num_channels * sample_rate];

            while (reader.ReadSamples(buff, 0, buff.Length) > 0)
                data.AddRange(buff);

            return new Sound<float>(data.ToArray(), num_channels, bits_per_sample, sample_rate);
        }

        internal static Sound<byte> LoadMP3(string file)
        {
            MpegFile mp3Stream = new MpegFile(file);
            int num_channels = mp3Stream.Channels;
            int bits_per_sample = 16;
            int sample_rate = mp3Stream.SampleRate;
            List<byte> data = new List<byte>();
            byte[] buff = new byte[num_channels * sample_rate * bits_per_sample / 8];

            while (mp3Stream.ReadSamples(buff, 0, buff.Length) > 0)
                data.AddRange(buff);

            return new Sound<byte>(data.ToArray(), num_channels, bits_per_sample, sample_rate);
        }

        internal static Sound<byte> LoadWave(string file)
        {
            Stream stream = File.Open(file, FileMode.Open);

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException($"Specified stream is not a wave file. Signature : {signature}");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException($"Specified stream is not a wave file. Format : {format}");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException($"Specified wave file is not supported. Format Signature : {format_signature}");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException($"Specified wave file is not supported. Data Signature : {data_signature}");

                int data_chunk_size = reader.ReadInt32();

                return new Sound<byte>(reader.ReadBytes((int)reader.BaseStream.Length), num_channels, bits_per_sample, sample_rate);
            }
        }
    }
}
