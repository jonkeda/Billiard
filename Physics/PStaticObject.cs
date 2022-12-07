using Physics.Colliders;
using System.Collections.Generic;
using Utilities;

namespace Physics
{
    class PStaticObject
    {
        public List<LodGroup> lodGroups = new List<LodGroup>();

        public virtual bool Collides(Vector2D p, double r = 0, bool ignoreLodGroups = false)
        {
            return MinDistance(p, r, ignoreLodGroups) < r;
        }

        public double MinDistance(Vector2D p, double r = 0, bool ignoreLodGroups = false) // Raymarching has to ignore the LodGroups to function properly
        {
            double distance = double.PositiveInfinity;

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