using System.Collections.Generic;
using System.Numerics;
using Billiard.Physics.Colliders;

namespace Billiard.Physics
{
    public class PStaticObject
    {
        public List<LodGroup> lodGroups = new List<LodGroup>();

        public virtual bool Collides(Vector2 p, float r = 0, bool ignoreLodGroups = false)
        {
            return MinDistance(p, r, ignoreLodGroups) < r;
        }

        public float MinDistance(Vector2 p, float r = 0, bool ignoreLodGroups = false) // Raymarching has to ignore the LodGroups to function properly
        {
            float distance = float.PositiveInfinity;

            if (ignoreLodGroups)
            {
                return lodGroups[^1].MinDistance(p);
            }

            foreach (LodGroup lodGroup in lodGroups)
            {
                distance = lodGroup.MinDistance(p);

                if (distance > r) return distance;
            }

            return distance;
        }

        public void AddLodGroup(LodGroup lodGroup)
        {
            lodGroups.Add(lodGroup);
        }
    }
}