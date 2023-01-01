using System.Collections.Generic;

namespace Billiard.Camera.vision
{
    public class CappedQueue<T>
    {
        public List<T> elements = new List<T>();
        private int capSize;
        public CappedQueue() : this(100)
        { }

        public CappedQueue(int capSize)
        {
            this.capSize = capSize;
        }

        public CappedQueue<T> push(T newElement)
        {
            elements.Add(newElement);
            if (elements.Count > capSize)
                elements = new List<T>();
            return this;
        }

        public CappedQueue<T> leftShift(T newElement)
        {
            return push(newElement);
        }

        public int size()
        {
            return elements.Count;
        }

/*        public String toString()
        {
            return elements.toString();

        }
*/
/*        public static void main(String[] args)
        {
            CappedQueue l = new CappedQueue(3);

            l << 1 << 2 << 3 << 4;
    
            println l
    
        }
*/    }
}
