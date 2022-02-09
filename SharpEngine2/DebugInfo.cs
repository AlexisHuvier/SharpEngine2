using System;
using System.Collections.Generic;

namespace SE2
{
    public class DebugInfo
    {
        public static Version SE2_VERSION = new Version(1, 0, 1);
        public static Version GL_VERSION = null;
        public static Version GLSL_VERSION = null;
        public static string RENDERER_NAME = null;
        public static string RENDERER_VENDOR = null;
        public static List<string> SUPPORTED_EXTENSIONS = new List<string>();
        public static Version AL_VERSION = null;
        public static string AL_RENDERER = null;
        public static string AL_VENDOR = null;
        public static Version ALC_VERSION = null;

        private static int frameRate = 0;
        private static int frameCounter = 0;
        private static double elapsedTime = 0;

        public static int GetFPS()
        {
            return frameRate;
        }

        public static long GetGCMemory()
        {
            return GC.GetTotalMemory(false);
        }

        internal static void Update(double gameTime)
        {
            elapsedTime += gameTime;

            if (elapsedTime > 1)
            {
                elapsedTime -= 1;
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        internal static void Draw()
        {
            frameCounter++;
        }
    }
}
