﻿using SE2;
using SE2.Utils;
using SE2.Widgets;

namespace SE2Test
{
    class MyWorld: Scene
    {
        public MyWorld(): base()
        {
            AddWidget(new Checkbox(new Vec3(200, 200, 0), new Vec3(50, 50, 1)));
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
