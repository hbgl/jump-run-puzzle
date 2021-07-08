using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpRunPuzzle.Support
{
    public static class ListExtensions
    {
        public static void SwapRemoveAt<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        public static bool SwapRemove<T>(this List<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index >= 0)
            {
                list.SwapRemoveAt(index);
                return true;
            }
            return false;
        }

        public static bool AddIfNotExists<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }
    }
}
