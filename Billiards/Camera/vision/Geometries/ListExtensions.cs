using System.Collections.Generic;
using Emgu.CV.Stitching;

namespace Billiard.Camera.vision.Geometries
{
    public static class ListExtensions
    {
        public static int size<T>(this List<T> list)
        {
            return list.Count;
        }
    }
}
