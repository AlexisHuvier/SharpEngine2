using System.Collections.Generic;

namespace SE2.Components
{
    public class Component
    {
        internal List<Entities.Entity> entities;

        public Component()
        {
            entities = new List<Entities.Entity>();
        }

        public Window GetWindow(int eID = 0)
        {
            if (entities.Count != 0 && entities[eID].scene != null)
                return entities[eID].scene.window;
            return null;
        }

        public virtual List<Entities.Entity> GetEntities() => entities;
        public virtual void AddEntity(Entities.Entity e) => entities.Add(e);
        public virtual void RemoveEntity(Entities.Entity e) => entities.Remove(e);
        public virtual void Init() { }
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void Update(double deltaTime) { }
        public virtual void Render() { }
    }
}
