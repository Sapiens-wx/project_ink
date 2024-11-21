using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int gridWidth, gridHeight;
    public MapConfig mapConfig;
    List<int>[][] grid;

    //debug
    Vector2Int position;

    private void AffectGrid(HashHeap<List<int>> heap, List<int> affectedGrid, int[] arr, MapConfig.Element element)
    {
        if (affectedGrid.Count > 1)
        {
            element.Collapse(affectedGrid, arr);
            heap.HeapifyUp(heap.dictionary[affectedGrid]);
        }
    }
    private void PrintDictionary(HashHeap<List<int>> heap)
    {
        StringBuilder sb = new StringBuilder();
        for(int j = 0; j < gridHeight; ++j)
        {
            for(int i = 0; i < gridWidth; ++i)
            {
                sb.Append(heap.dictionary.ContainsKey(grid[i][j]) ? '1' : '0');
                sb.Append(' ');
            }
            sb.Append('\n');
        }
        Debug.Log(sb);
    }
    private void PrintGrid()
    {
        StringBuilder sb = new StringBuilder(300);
        StringBuilder buffer = new StringBuilder(60);
        for(int j = 0; j < gridHeight; ++j)
        {
            for (int i = 0; i < gridWidth; ++i)
            {
                buffer.Clear();
                buffer.Append('[');
                foreach (int k in grid[i][j])
                    buffer.Append(k.ToString() + ',');
                buffer[buffer.Length - 1] = ']';
                while (buffer.Length < 12)
                    buffer.Append(' ');
                sb.Append(buffer);
            }
            sb.Append('\n');
        }
        Debug.Log(sb.ToString());
    }
    private void AffectSurroundingTiles(HashHeap<List<int>> heap, Vector2Int position, MapConfig.Element element)
    {
        if (position.y > 0) { //--->top horizontal line
            if (position.x > 0) { //left
                AffectGrid(heap, grid[position.x - 1][position.y - 1], element.ltTop, element);
                //tmp = grid[position.x - 1][position.y - 1];
                //if (tmp.Count > 1) {
                //    element.Collapse(tmp, element.ltTop);
                //    heap.HeapifyUp(heap.dictionary[tmp]);
                //}
            }
            //middle
            AffectGrid(heap, grid[position.x][position.y - 1], element.midTop, element);
            //tmp = grid[position.x][position.y - 1];
            //if(tmp.Count>1) {
            //    element.Collapse(tmp, element.midTop);
            //    heap.HeapifyUp(heap.dictionary[tmp]);
            //}
            //right
            if (position.x < gridWidth - 1) { //right
                AffectGrid(heap, grid[position.x + 1][position.y - 1], element.rtTop, element);
                //tmp = grid[position.x + 1][position.y - 1];
                //if(tmp.Count>1) {
                //    element.Collapse(tmp, element.rtTop);
                //    heap.HeapifyUp(heap.dictionary[tmp]);
                //}
            }
        }
        //--->middle horizontal line
        //left
        if (position.x > 0) {
            AffectGrid(heap, grid[position.x - 1][position.y], element.ltMid, element);
            //tmp = grid[position.x - 1][position.y];
            //if(tmp.Count>1) {
            //    element.Collapse(tmp, element.ltMid);
            //    heap.HeapifyUp(heap.dictionary[tmp]);
            //}
        }
        //right
        if (position.x < gridWidth - 1) { //right
            AffectGrid(heap, grid[position.x + 1][position.y], element.rtMid, element);
            //tmp = grid[position.x + 1][position.y];
            //if(tmp.Count>1) {
            //    element.Collapse(tmp, element.rtMid);
            //    heap.HeapifyUp(heap.dictionary[tmp]);
            //}
        }
        //--->bottom horizontal line
        if (position.y < gridHeight - 1) {
            if (position.x > 0) { //left
                AffectGrid(heap, grid[position.x - 1][position.y + 1], element.ltBot, element);
                //tmp = grid[position.x - 1][position.y + 1];
                //if(tmp.Count>1) {
                //    element.Collapse(tmp, element.ltBot);
                //    heap.HeapifyUp(heap.dictionary[tmp]);
                //}
            }
            //middle
            AffectGrid(heap, grid[position.x][position.y + 1], element.midBot, element);
            //tmp = grid[position.x][position.y + 1];
            //if(tmp.Count>1) {
            //    element.Collapse(tmp, element.midBot);
            //    heap.HeapifyUp(heap.dictionary[tmp]);
            //}
            //right
            if (position.x < gridWidth - 1) { //right
                AffectGrid(heap, grid[position.x + 1][position.y + 1], element.rtBot, element);
                //tmp = grid[position.x + 1][position.y + 1];
                //if(tmp.Count>1) {
                //    element.Collapse(tmp, element.rtBot);
                //    heap.HeapifyUp(heap.dictionary[tmp]);
                //}
            }
        }

    }
    private void ReduceAndCheckPickedElementCount(HashHeap<List<int>> heap, int[] counts, int pickedElement)
    {
        if (--counts[pickedElement] <= 0)
        {
            int i = 0, tmp;
            for (; i < heap.Count && heap.Get(i).Count == 1; ++i) ;
            for(; i < heap.Count; ++i)
            {
                tmp = heap.Get(i).BinarySearch(pickedElement);
                if (tmp >= 0) {
                    heap.Get(i).RemoveAt(tmp);
                    heap.HeapifyUp(i);
                }
            }
        }
        
    }
    [ContextMenu("test")]
    void InitGrid()
    {
        mapConfig.Initialize();
        //��ÿ��Ԫ�����ɵĸ���
        int[] counts=new int[mapConfig.arr.Count]; 
        for(int i = 0; i < counts.Length; ++i) {
            counts[i] = mapConfig.arr[i].maxAmount == 0 ? int.MaxValue : mapConfig.arr[i].maxAmount;
        }
        //initialize grid
        /*
         * 0: empty
         * 1: 1x1
         * 2: 2x1 left
         * 3: 2x1 right
         * 4: 1x2 up
         * 5: 1x2 down
         * 6: 4x4 left top
         * 7: 4x4 right top
         * 8: 4x4 left bottom
         * 9: 4x4 right bottom
        */
        grid = new List<int>[gridWidth][];
        for(int i = 0; i < gridWidth; i++)
        {
            grid[i] = new List<int>[gridHeight];
        }
        HashHeap<List<int>> heap=new HashHeap<List<int>>((List<int> l, List<int> r) => { return l.Count < r.Count; });
        Dictionary<List<int>, Vector2Int> dictionary=new Dictionary<List<int>, Vector2Int>(heap.Count);
        grid[0][0] = new List<int>(mapConfig.ltTopTiles); //left top tiles
        for(int i = gridHeight - 2; i > 0; --i) { // left mid tiles
            grid[0][i] = new List<int>(mapConfig.ltMidTiles);
        }
        grid[0][gridHeight - 1] = new List<int>(mapConfig.ltBotTiles); //left bot tiles
        for (int i = gridWidth - 2; i > 0; --i) //mid top tiles
            grid[i][0] = new List<int>(mapConfig.midTopTiles);
        grid[gridWidth - 1][0] = new List<int>(mapConfig.rtTopTiles); //right top tiles
        for (int i = gridWidth - 2, j = gridHeight - 1; i > 0; --i)
            grid[i][j] = new List<int>(mapConfig.midBotTiles);//mid bot tiles
        for (int i = gridWidth - 1, j = gridHeight - 2; j > 0; --j)//right mid tiles
            grid[i][j] = new List<int>(mapConfig.rtMidTiles);
        grid[gridWidth - 1][gridHeight - 1] = new List<int>(mapConfig.rtBotTiles); //right bot tiles
        for(int i = gridWidth - 2; i > 0; --i)//mid mid tiles
            for(int j = gridHeight - 2; j > 0; --j)
                grid[i][j] = new List<int>(mapConfig.initialTiles);
        for(int i = 0; i < gridWidth; i++) {
            for(int j = 0; j < gridHeight; j++) {
                heap.Insert(grid[i][j]);
                dictionary[grid[i][j]] = new Vector2Int(i, j);
            }
        }
        //wave collapse loop
        List<List<int>> minTiles = new List<List<int>>(heap.Count);
        List<int> cur;
        int pickedNumber;
        int minTilesCount;
        int randomN;
        while (heap.Count>0)
        {
            minTilesCount = heap.Front().Count;
            //find tiles with the least number of choices
            while(heap.Count>0 && heap.Front().Count == minTilesCount)
            {
                minTiles.Add(heap.Pop());
            }
            //randomly pick one tile to collapse it
            randomN = UnityEngine.Random.Range(0, minTiles.Count);
            cur = minTiles[randomN];
            for(int i = 0; i < minTiles.Count; ++i)//move unselected tiles back to the heap
            {
                if (i != randomN) heap.Insert(minTiles[i]);
            }
            pickedNumber = cur[UnityEngine.Random.Range(0, cur.Count)];
            cur.Clear();
            cur.Add(pickedNumber);
            //count picked element
            ReduceAndCheckPickedElementCount(heap, counts, pickedNumber);
            //reduce number choices of other tiles
            AffectSurroundingTiles(heap, dictionary[cur], mapConfig.arr[pickedNumber]);
            minTiles.Clear();
            while (heap.Count > 0 && heap.Front().Count == 1)
            {
                ReduceAndCheckPickedElementCount(heap, counts, heap.Front()[0]);
                AffectSurroundingTiles(heap, dictionary[heap.Front()], mapConfig.arr[heap.Front()[0]]);
                heap.Pop();
            }
        }
        PrintGrid();
    }
}

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
