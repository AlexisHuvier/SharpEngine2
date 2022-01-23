using SE2;
using SE2.Entities;
using SE2.Components;
using SE2.Widgets;
using SE2.Utils;
using System.Collections.Generic;

namespace SE2Test
{
    class MyWorld: Scene
    {
        readonly Entity e5;
        readonly Entity e6;

        public MyWorld(): base()
        {
            Entity e2 = new Entity();
            e2.AddComponent(new TransformComponent(new Vec3(600, 450)));
            e2.AddComponent(new TileMapComponent("tiled/map.tmx"));
            AddEntity(e2);

            e5 = new Entity();
            e5.AddComponent(new TransformComponent(new Vec3(600, 450), new Vec3(3)));
            e5.AddComponent(new SpriteSheetComponent("spritesheet", new Vec2(32), new Dictionary<string, List<int>>() { { "idle", new List<int>() { 13, 14, 15, 16, 17, 18, 19, 20 } } }, "idle", 150));
            e5.AddComponent(new ControlComponent(ControlType.FOURDIRECTION));
            AddEntity(e5);

            Entity e7 = new Entity();
            e7.AddComponent(new TransformComponent(new Vec3(600, 450), 10));
            e7.AddComponent(new RectComponent(new Vec3(400, 100), Color.BLUE));
            e7.AddComponent(new PhysicsComponent(new Vec2(400, 100), 1, bodyType: tainicom.Aether.Physics2D.Dynamics.BodyType.Kinematic));
            AddEntity(e7);

            e6 = new Entity();
            e6.AddComponent(new TransformComponent(new Vec3(600, 700), 35));
            e6.AddComponent(new CircleComponent(20f, Color.RED, 20));
            e6.AddComponent(new PhysicsComponent(20f, density: 10f));
            AddEntity(e6);

            AddWidget(new Image(new Vec3(100, 100), new Vec3(1), "awesomeface"));
            AddWidget(new Label(new Vec3(100, 100), new Vec3(1), "Heyo !", "basic"));
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.X))
                e5.GetComponent<SpriteSheetComponent>().flipX = !e5.GetComponent<SpriteSheetComponent>().flipX;
            if (GetWindow().inputManager.IsKeyPressed(Inputs.Key.Y))
                e5.GetComponent<SpriteSheetComponent>().flipY = !e5.GetComponent<SpriteSheetComponent>().flipY;
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

            if (GetWindow().inputManager.IsMouseButtonPressed(Inputs.MouseButton.LEFT))
                e6.GetComponent<PhysicsComponent>().SetPosition(GetWindow().inputManager.GetMousePosition());
        }

        public override void Load()
        {
            base.Load();

            GetWindow().GetCamera().follow = e5;
        }
    }
}
