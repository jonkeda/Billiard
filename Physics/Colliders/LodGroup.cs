using System;
using System.Collections.Generic;
using Utilities;

namespace Physics
{
    namespace Colliders
    {
        class LodGroup
        {
            private readonly List<Collider> colliders = new List<Collider>();

            public void AddCollider(Collider collider)
            {
                colliders.Add(collider);
            }

            public double MinDistance(Vector2D p)
            {
                double d = double.PositiveInfinity;

                foreach (Collider collider in colliders)
                {
                    switch (collider.mode)
                    {
                        case ColliderMode.Union:
                            d = Math.Min(d, collider.MinDistance(p));
                            break;
                        case ColliderMode.Subtract:
                            d = SDFOp.Subtraction(collider.MinDistance(p), d);
                            break;
                        case ColliderMode.Intersect:
                            d = SDFOp.Intersection(d, collider.MinDistance(p));
                            break;
                        case ColliderMode.SmoothUnion:
                            d = SDFOp.SmoothUnion(d, collider.MinDistance(p), collider.k);
                            break;
                        case ColliderMode.SmoothSubtract:
                            d = SDFOp.SmoothSubtraction(collider.MinDistance(p), d, collider.k);
                            break;
                        case ColliderMode.SmoothIntersect:
                            d = SDFOp.SmoothIntersection(d, collider.MinDistance(p), collider.k);
                            break;
                        case ColliderMode.Negate:
                            d = Math.Min(d, -collider.MinDistance(p));
                            break;
                    }
                }

                return d;
            }
        }
    }
}
