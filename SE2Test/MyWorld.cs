﻿using SE2;
using SE2.Utils;
using SE2.Widgets;
using SE2.Entities;
using SE2.Components;

namespace SE2Test
{
    class MyWorld: Scene
    {
        public MyWorld(): base()
        {
            Entity e = new Entity();
            e.AddComponent(new TransformComponent(new Vec3(700, 400), new Vec3(1)));
            e.AddComponent(new SpriteAnimComponent(new string[] { "awesomeface", "container" }));
            AddEntity(e);

            AddWidget(new Button(new Vec3(200, 200), new Vec3(200, 50), "HEYO", "basic", null));
            AddWidget(new TexturedButton(new Vec3(200, 400), new Vec3(200, 50), "HEYO", "basic", new string[] { "awesomeface", "container", "container" }, null));
            AddWidget(new ProgressBar(new Vec3(200, 600), new Vec3(200, 50), Color.AQUA, 75));
            AddWidget(new TexturedProgressBar(new Vec3(500, 600), new Vec3(200, 50), new string[] { "awesomeface", "container" }));
            AddWidget(new LineEdit(new Vec3(550, 400), new Vec3(300, 50), "Heyo", "basic"));
            AddWidget(new Checkbox(new Vec3(500, 200), new Vec3(50, 50)));
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
