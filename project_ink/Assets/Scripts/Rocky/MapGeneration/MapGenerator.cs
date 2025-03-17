using System.Collections.Generic;

public class HashHeap<T>{
    private List<T> list;
    private Compare compare;
    public Dictionary<T, int> dictionary;

    public delegate bool Compare(T lhs, T rhs); //true: lhs should be before rhs
    public HashHeap(Compare compareInterface)
    {
        list = new List<T>();
        compare = compareInterface;
        dictionary=new Dictionary<T, int>();
    }
    public int Count
    {
        get { return list.Count; }
    }
    public T Get(int index)
    {
        return list[index];
    }
    private void SwapElement(int l, int r)
    {
        T temp = list[l];
        list[l] = list[r];
        list[r] = temp;
        dictionary[list[l]] = l;
        dictionary[temp] = r;
    }
    public void Insert(T item)
    {
        int cur = list.Count, parent=(cur-1)>>1;
        dictionary.Add(item, cur);
        list.Add(item);
        while(cur>0 && compare(list[cur], list[parent]))
        {
            SwapElement(cur, parent);
            cur = parent;
            parent = (cur - 1) >> 1;
        }
    }
    public T Front()
    {
        return list[0];
    }
    public T Pop()
    {
        T ret = list[0];
        dictionary.Remove(ret);
        list[0] = list[list.Count - 1];
        RemoveAt(list.Count - 1);
        if (list.Count > 0) {
            dictionary.Add(list[0], 0);
            HeapifyDown(0);
        }
        return ret;
    }
    public void Remove(T item)
    {
        RemoveAt(dictionary[item]);
    }
    public void RemoveAt(int index)
    {
        dictionary.Remove(list[index]);
        list[index] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        if (index >= list.Count)
            return;
        dictionary[list[index]]=index;
        HeapifyDown(index);
    }
    private void HeapifyDown(int cur)
    {
        int smaller = cur;
        int l = (cur << 1) | 1, r = l + 1;
        while (l<list.Count)
        {
            if (compare(list[l], list[smaller])) smaller = l;
            if (r<list.Count && compare(list[r], list[smaller])) smaller = r;
            if (smaller == cur) break;
            SwapElement(cur, smaller);
            cur = smaller;
            l = (cur << 1) | 1;
            r = l + 1;
        }
    }
    public void HeapifyUp(int cur)
    {
        int parent = (cur - 1) >> 1;
        while(parent>-1 && compare(list[cur], list[parent]))
        {
            SwapElement(cur, parent);
            cur = parent;
            parent = (cur - 1) >> 1;
        }
    }
}
