using SE2;
using SE2.Utils;
using SE2.Widgets;
using SE2.Entities;
using SE2.Components;
using System.Collections.Generic;

namespace SE2Test
{ 
    class MyWorld: Scene
    {
        Entity e2;

        public MyWorld() : base()
        {
            Entity e = new Entity();
            e.AddComponent(new TransformComponent(new Vec3(700, 400), new Vec3(3)));
            e.AddComponent(new TileMapComponent("tiled/map.tmx"));
            AddEntity(e);

            e2 = new Entity();
            e2.AddComponent(new TransformComponent(new Vec3(700, 400), new Vec3(1)));
            e2.AddComponent(new SpriteAnimComponent(new Dictionary<string, List<string>>()
            {
                { "basic", new List<string>() { "container", "awesomeface" } }
            }, "basic"));
            e2.AddComponent(new ControlComponent());
            AddEntity(e2);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.M))
                GetWindow().soundManager.PlaySound("mp3");
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.P))
                GetWindow().soundManager.PauseSound("mp3");
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.S))
                System.Console.WriteLine(GetWindow().soundManager.GetState("mp3"));
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.O))
                GetWindow().soundManager.PlaySound("ogg");
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.E))
                GetWindow().soundManager.StopSound("ogg");
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.W))
                GetWindow().soundManager.PlaySound("wav");

        }

        public override void Load()
        {
            base.Load();
            GetWindow().GetCamera().follow = e2;
        }
    }
}
