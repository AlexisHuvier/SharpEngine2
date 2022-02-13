using SE2;
using SE2.Utils;
using SE2.Widgets;
using SE2.Entities;
using SE2.Components;

namespace SE2Test
{ 
    class MyWorld: Scene
    {

        public MyWorld() : base()
        {
            Entity e = new Entity();
            e.AddComponent(new TransformComponent(new Vec3(700, 400), new Vec3(2)));
            e.AddComponent(new TileMapComponent("tiled/map.tmx"));
            AddEntity(e);
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
