using System;
using System.Collections.Generic;

namespace SE2.Components
{
    public class RectCollisionComponent: Component
    {
        public Utils.Vec3 size;
        public Utils.Vec3 offset;
        public bool solid;
        public Action<List<Entities.Entity>, List<Entities.Entity>, string> collisionCallback;

        public RectCollisionComponent(Utils.Vec3 siz, Utils.Vec3 off, bool sol) : base()
        {
            size = siz;
            offset = off;
            solid = sol;
            collisionCallback = null;
        }

        public bool CanGo(Utils.Vec3 position, string cause)
        {
            var rect = new Utils.Rect(position.x + offset.x, position.y + offset.y, position.z + offset.z, size.x, size.y, size.z);
            foreach (Entities.Entity e in entities[0].scene.GetEntities())
            {
                if (!entities.Contains(e))
                {
                    if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                    {
                        if (e.GetComponent<RectCollisionComponent>() is RectCollisionComponent rcc)
                        {
                            var erect = new Utils.Rect(tc.position.x + rcc.offset.x, tc.position.y + rcc.offset.y, tc.position.z + rcc.offset.z, rcc.size.x, rcc.size.y, rcc.size.z);
                            if (erect.Intersect(rect))
                            {
                                if (collisionCallback != null)
                                    collisionCallback(entities, new List<Entities.Entity>() { e }, cause);
                                if (rcc.collisionCallback != null)
                                    collisionCallback(new List<Entities.Entity>() { e }, entities, cause);
                                if (solid && rcc.solid)
                                    return false;
                            }
                        }
                        else if (e.GetComponent<SphereCollisionComponent>() is SphereCollisionComponent scc)
                        {
                            var esphere = new Utils.Sphere(tc.position.x + scc.offset.x, tc.position.y + scc.offset.y, tc.position.z + scc.offset.z, scc.radius);
                            if (esphere.Intersect(rect))
                            {
                                if (collisionCallback != null)
                                    collisionCallback(entities, new List<Entities.Entity>() { e }, cause);
                                if (scc.collisionCallback != null)
                                    collisionCallback(new List<Entities.Entity>() { e }, entities, cause);
                                if (solid && scc.solid)
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public override string ToString()
        {
            return $"RectCollisionComponent(size={size}, offset={offset})";
        }

    }
}
