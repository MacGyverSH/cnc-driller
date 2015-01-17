using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public class GQueue<T> : Queue<T>
    {
        private object _lock = new object();

        public int QueueLength
        {
            get
            {
                lock (_lock)
                {
                    return Count;
                }
            }
        }

        public new void Enqueue(T d)
        {
            lock (_lock)
            {
                base.Enqueue(d);
            }
        }

        public new T Dequeue()
        {
            lock(_lock){
                if (base.Count > 0)
                {
                    return base.Dequeue();
                }
                return default(T);
            }
        }

    }
}
