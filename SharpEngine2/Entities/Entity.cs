using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Entities
{
    public class Entity
    {
        internal List<Components.Component> components;
        internal Scene scene;
        public int id;
        internal List<string> tags;


        public Entity()
        {
            components = new List<Components.Component>();
            tags = new List<string>();
            scene = null;
            id = -1;
        }

        public virtual void SetScene(Scene w) => scene = w;
        public virtual Scene GetScene() => scene;

        public bool HasTag(string tag) => tags.Contains(tag);
        public void AddTag(string tag) => tags.Add(tag);
        public void RemoveTag(string tag) => tags.Remove(tag);

        public Window GetWindow()
        {
            if (scene != null)
                return scene.window;
            return null;
        }

        public virtual bool HasComponent<T>() where T: Components.Component
        {
            foreach (Components.Component comp in components)
            {
                if (comp.GetType() == typeof(T))
                    return true;
            }
            return false;
        }

        public virtual T GetComponent<T>() where T: Components.Component
        {
            foreach(Components.Component comp in components)
            {
                if (comp.GetType() == typeof(T))
                    return (T)comp;
            }
            return null;
        }

        public virtual T AddComponent<T>(T component) where T: Components.Component
        {
            foreach(Components.Component comp in components)
            {
                if (comp.GetType() == typeof(T))
                {
                    Trace.WriteLine($"[ERROR] Entity already have {typeof(T)}");
                    return null;
                }
            }
            component.AddEntity(this);
            components.Add(component);
            return component;
        }

        public virtual T RemoveComponent<T>() where T: Components.Component
        {
            foreach (Components.Component comp in components)
            {
                if (comp.GetType() == typeof(T))
                {
                    comp.RemoveEntity(this);
                    components.Remove(comp);
                    return (T) comp;
                }
            }
            Trace.WriteLine($"[ERROR] Entity doesn't have {typeof(T)}");
            return null;
        }

        public virtual void Init() 
        {
            foreach (Components.Component comp in components)
                comp.Init();
        }

        public virtual void Load()
        {
            foreach (Components.Component comp in components)
                comp.Load();
        }

        public virtual void Unload()
        {
            foreach (Components.Component comp in components)
                comp.Unload();
        }

        public virtual void Update(double deltaTime)
        {
            foreach (Components.Component comp in components)
                comp.Update(deltaTime);
        }

        public virtual void Render()
        {
            foreach (Components.Component comp in components)
                comp.Render();
        }
    }
}
