using SE2;

namespace SE2Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Window win = new Window(1200, 900, "SharpEngine 2 INCOMING !", true, false);

            win.fontManager.AddFont("basic", "Resources/basic.ttf", 50);

            win.soundManager.AddSound("test", "Resources/tuturu_1.wav", SE2.Utils.SoundType.WAV);

            win.textureManager.AddTexture("container", "Resources/container.png");
            win.textureManager.AddTexture("awesomeface", "Resources/awesomeface.png");
            win.textureManager.AddTexture("spritesheet", "Resources/spritesheet.png");
            win.textureManager.AddTexture("temp", "Resources/temp.gif");

            win.soundManager.PlaySound("test");

            win.AddScene(new MyWorld());
            win.Run();
        }
    }
}
