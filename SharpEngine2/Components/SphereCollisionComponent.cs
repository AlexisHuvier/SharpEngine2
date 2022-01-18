using System;
using System.Collections.Generic;
using System.Text;

namespace SE2.Components
{
    public class SphereCollisionComponent: Component
    {
        public float radius;
        public Utils.Vec3 offset;
        public bool solid;
        public Action<List<Entities.Entity>, List<Entities.Entity>, string> collisionCallback;

        public SphereCollisionComponent(float rad, Utils.Vec3 off, bool sol) : base()
        {
            radius = rad;
            offset = off;
            solid = sol;
            collisionCallback = null;
        }

        public bool CanGo(Utils.Vec3 position, string cause)
        {
            var sphere = new Utils.Sphere(position.x + offset.x, position.y + offset.y, position.z + offset.z, radius);
            foreach (Entities.Entity e in entities[0].scene.GetEntities())
            {
                if (!entities.Contains(e))
                {
                    if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                    {
                        if (e.GetComponent<SphereCollisionComponent>() is SphereCollisionComponent scc)
                        {
                            var esphere = new Utils.Sphere(tc.position.x + scc.offset.x, tc.position.y + scc.offset.y, tc.position.z + scc.offset.z, scc.radius);
                            if (esphere.Intersect(sphere))
                            {
                                if (collisionCallback != null)
                                    collisionCallback(entities, new List<Entities.Entity>() { e }, cause);
                                if (scc.collisionCallback != null)
                                    collisionCallback(new List<Entities.Entity>() { e }, entities, cause);
                                if (solid && scc.solid)
                                    return false;
                            }
                        }
                        else if (e.GetComponent<RectCollisionComponent>() is RectCollisionComponent rcc)
                        {
                            var erect = new Utils.Rect(tc.position.x + rcc.offset.x, tc.position.y + rcc.offset.y, tc.position.z + rcc.offset.z, rcc.size.x, rcc.size.y, rcc.size.z);
                            if (erect.Intersect(sphere))
                            {
                                if (collisionCallback != null)
                                    collisionCallback(entities, new List<Entities.Entity>() { e }, cause);
                                if (rcc.collisionCallback != null)
                                    collisionCallback(new List<Entities.Entity>() { e }, entities, cause);
                                if (solid && rcc.solid)
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
            return $"SphereCollisionComponent(radius={radius}, offset={offset})";
        }
    }
}
