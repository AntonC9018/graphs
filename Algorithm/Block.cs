
using System;
using System.Collections.Generic;

namespace Algorithm
{
    public class Block
    {
        private List<HashSet<int>> columns;
        private Queue<int> unused;

        public void RefillUnused()
        {
            unused.Clear();
            for (int i = 0; i < columns.Count; i++)
                unused.Enqueue(i);
        }

        public Block()
        {
            columns = new List<HashSet<int>>();
            unused = new Queue<int>();
        }

        public void AppendColumn(HashSet<int> column)
        {
            unused.Enqueue(columns.Count);
            columns.Add(column);
        }

        public (int, HashSet<int>) GetNextColumn()
        {
            int index = unused.Dequeue();
            var column = columns[index];
            return (index, column);
        }

        public bool AreAllColumnsUsed()
        {
            return unused.Count == 0;
        }

        public void Print()
        {
            foreach (var column in columns)
            {
                foreach (int i in column)
                {
                    System.Console.Write($"{i}, ");
                }
                System.Console.WriteLine();
            }
        }
    }
}