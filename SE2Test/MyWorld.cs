using SE2;
using SE2.Entities;
using SE2.Components;
using SE2.Utils;

namespace SE2Test
{
    class MyWorld: Scene
    {
        public MyWorld(): base()
        {
            Entity e8 = new Entity();
            e8.AddComponent(new TransformComponent(new Vec3(600, 700), 0));
            e8.AddComponent(new PolygonComponent(new float[] { 
                50f, 50f, 0f, 
                -50f, -50f, 0f, 
                -50f, 50f, 0f 
            }, Color.RED));
            AddEntity(e8);

            Entity e9 = new Entity();
            e9.AddComponent(new TransformComponent(new Vec3(600, 600), 0));
            e9.AddComponent(new PolygonComponent(new float[] {
                50f, 50f, 0f,
                50f, -50f, 0f,
                -50f, -50f, 0f,
                -50f, 50f, 0f
            }, Color.BLUE));
            AddEntity(e9);

            Entity e10 = new Entity();
            e10.AddComponent(new TransformComponent(new Vec3(600, 500), 0));
            e10.AddComponent(new PolygonComponent(new float[] {
                50f, 50f, 0f,
                75f, 0f, 0f, 
                50f, -50f, 0f,
                -50f, -50f, 0f,
                -50f, 50f, 0f
            }, Color.GREEN));
            AddEntity(e10);
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
