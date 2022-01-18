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

        public MyWorld(): base()
        {
            Entity e2 = new Entity();
            e2.AddComponent(new TransformComponent(new Vec3(600, 450)));
            e2.AddComponent(new TileMapComponent("tiled/map.tmx"));
            AddEntity(e2);

            e5 = new Entity();
            e5.AddComponent(new TransformComponent(new Vec3(600, 450), new Vec3(1)));
            e5.AddComponent(new SpriteSheetComponent("spritesheet", new Vec2(32), new Dictionary<string, List<int>>() { { "idle", new List<int>() { 13, 14, 15, 16, 17, 18, 19, 20 } } }, "idle", 150));
            //e5.AddComponent(new SpriteComponent("awesomeface"));
            e5.AddComponent(new ControlComponent(ControlType.FOURDIRECTION));
            e5.AddComponent(new RectCollisionComponent(new Vec3(96, 96, 1), new Vec3(0), true));
            AddEntity(e5);

            Entity e4 = new Entity();
            e4.AddComponent(new TransformComponent(new Vec3(600, 450)));
            e4.AddComponent(new TextComponent("SALUT !", "basic"));
            AddEntity(e4);

            AddWidget(new Image(new Vec3(100, 100), new Vec3(1), "awesomeface"));
            AddWidget(new Label(new Vec3(100, 100), new Vec3(1), "Heyo !", "basic"));
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (SE2.Managers.InputManager.IsKeyPressed(Inputs.Key.X))
                e5.GetComponent<SpriteSheetComponent>().flipX = !e5.GetComponent<SpriteSheetComponent>().flipX;
            if (SE2.Managers.InputManager.IsKeyPressed(Inputs.Key.Y))
                e5.GetComponent<SpriteSheetComponent>().flipY = !e5.GetComponent<SpriteSheetComponent>().flipY;
        }

        public override void Load()
        {
            base.Load();

            GetWindow().GetCamera().follow = GetEntities()[0];
        }
    }
}
