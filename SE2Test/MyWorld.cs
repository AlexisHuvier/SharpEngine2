﻿using SE2;
using SE2.Utils;
using SE2.Widgets;

namespace SE2Test
{
    class MyWorld: Scene
    {
        public MyWorld(): base()
        {
            AddWidget(new Button(new Vec3(200, 200, 0), new Vec3(200, 50, 1), "HEYO", "basic", null));
            AddWidget(new TexturedButton(new Vec3(200, 400, 0), new Vec3(200, 50, 1), "HEYO", "basic", new string[] { "awesomeface", "container", "container" }, null));
            AddWidget(new ProgressBar(new Vec3(200, 600, 0), new Vec3(200, 50), Color.AQUA, 75));
            AddWidget(new TexturedProgressBar(new Vec3(500, 600), new Vec3(200, 50), new string[] { "awesomeface", "container" }));
            AddWidget(new LineEdit(new Vec3(550, 400), new Vec3(300, 50), "Heyo", "basic"));
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.M))
            {
                System.Console.WriteLine("MP3");
                GetWindow().soundManager.PlaySound("mp3");
            }
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.O))
            {
                System.Console.WriteLine("OGG");
                GetWindow().soundManager.PlaySound("ogg");
            }
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.W))
            {
                System.Console.WriteLine("WAV");
                GetWindow().soundManager.PlaySound("wav");
            }
        }

        public override void Load()
        {
            base.Load();
        }
    }
}
