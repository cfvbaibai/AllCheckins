﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.CrawlerCli
{
    public class FixedCapacityPipeline <T>
    {
        private Queue <T> cancelQueue;
        public int Capacity { get; private set; }

        public FixedCapacityPipeline(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be positive integer.");
            }
            this.Capacity = capacity;
            this.cancelQueue = new Queue<T>(capacity);
        }

        public T Enqueue(T cancelled)
        {
            cancelQueue.Enqueue(cancelled);
            if (cancelQueue.Count > Capacity)
            {
                return cancelQueue.Dequeue();
            }
            return default(T);
        }

        public IList<T> ToList()
        {
            return this.cancelQueue.ToList();
        }
    }
}
