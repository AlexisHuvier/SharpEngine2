using System.Collections.Generic;

namespace SE2
{
    public class Scene
    {
        internal Window window;
        internal List<Entities.Entity> entities;
        internal List<Widgets.Widget> widgets;

        public Scene()
        {
            window = null;
            entities = new List<Entities.Entity>();
            widgets = new List<Widgets.Widget>();
        }

        public virtual List<Entities.Entity> GetEntities() => entities;

        public virtual void AddEntity(Entities.Entity e)
        {
            e.SetScene(this);
            entities.Insert(0, e);
        }

        public virtual void RemoveEntity(Entities.Entity e)
        {
            e.SetScene(null);
            entities.Remove(e);
        }

        public virtual List<Widgets.Widget> GetWidgets() => widgets;

        public virtual void AddWidget(Widgets.Widget w)
        {
            w.SetScene(this);
            widgets.Insert(0, w);
        }

        public virtual void RemoveWidget(Widgets.Widget w)
        {
            w.SetScene(null);
            widgets.Remove(w);
        }

        public virtual void SetWindow(Window window) => this.window = window;
        public virtual Window GetWindow() => window;

        public virtual void Init()
        {
            foreach (Entities.Entity e in entities)
                e.Init();

            foreach (Widgets.Widget w in widgets)
                w.Init();
        }

        public virtual void Load()
        {
            foreach (Entities.Entity e in entities)
                e.Load();

            foreach (Widgets.Widget w in widgets)
                w.Load();
        }

        public virtual void Unload()
        {
            foreach (Entities.Entity e in entities)
                e.Unload();

            foreach (Widgets.Widget w in widgets)
                w.Unload();
        }

        public virtual void Update(double deltaTime)
        {
            foreach (Entities.Entity e in entities)
                e.Update(deltaTime);

            foreach (Widgets.Widget w in widgets)
                w.Update(deltaTime);
        }

        public virtual void Render()
        {
            foreach (Widgets.Widget w in widgets)
                w.Render();

            foreach (Entities.Entity e in entities)
                e.Render();
        }
    }
}
