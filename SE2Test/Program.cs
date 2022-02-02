using SE2;

namespace SE2Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Window win = new Window(1200, 900, "SharpEngine 2 INCOMING !", true, false);

            win.fontManager.AddFont("basic", "Resources/basic.ttf", 30);

            win.soundManager.AddSound("wav", "Resources/tuturu_1.wav", SE2.Utils.SoundType.WAV);
            win.soundManager.AddSound("ogg", "Resources/tuturu_1.ogg", SE2.Utils.SoundType.OGG);
            win.soundManager.AddSound("mp3", "Resources/music.mp3", SE2.Utils.SoundType.MP3);

            win.textureManager.AddTexture("container", "Resources/container.png");
            win.textureManager.AddTexture("awesomeface", "Resources/awesomeface.png");
            win.textureManager.AddTexture("spritesheet", "Resources/spritesheet.png");
            win.textureManager.AddTexture("temp", "Resources/temp.gif");

            win.AddScene(new MyWorld());
            win.Run();
        }
    }
}
