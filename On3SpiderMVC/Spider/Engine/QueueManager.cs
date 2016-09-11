using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using Spider.Interface;

namespace Spider.Engine
{
    public class QueueManager<T> : IQueueManager<T>
    {
        private readonly ConcurrentQueue<T> _queue;

        public QueueManager()
        {
            _queue = new ConcurrentQueue<T>();
        }

        public QueueManager(IEnumerable<T> items)
        {
            _queue = new ConcurrentQueue<T>(items);
        }  

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            return _queue.TryDequeue(out item);
        }

        public bool IsEmpty()
        {
            return _queue.IsEmpty;
        }

        public int Count()
        {
            return _queue.Count;
        }
    }
}
