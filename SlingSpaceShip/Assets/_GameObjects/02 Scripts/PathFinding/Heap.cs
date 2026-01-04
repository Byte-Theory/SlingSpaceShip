using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int curItemCt;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.heapIdx = curItemCt;
        items[curItemCt] = item;
        SortUp(item);
        
        curItemCt++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        curItemCt--;
        
        items[0] = items[curItemCt];
        items[0].heapIdx = 0;
        
        SortDown(items[0]);
        
        return firstItem;
    }

    private void SortUp(T item)
    {
        int parentIdx = (curItemCt - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIdx];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            
            parentIdx = (curItemCt - 1) / 2;
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIdxLeft = item.heapIdx * 2 + 1;
            int childIdxRight = item.heapIdx * 2 + 2;

            if (childIdxLeft < curItemCt)
            {
                int swapIdx = childIdxLeft;

                if (childIdxRight < curItemCt)
                {
                    if (items[childIdxLeft].CompareTo(items[childIdxRight]) < 0)
                    {
                        swapIdx = childIdxRight;
                    }
                }

                if (item.CompareTo(items[swapIdx]) < 0)
                {
                    Swap(item, items[swapIdx]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.heapIdx] = itemB;
        items[itemB.heapIdx] = itemA;
        
        (itemA.heapIdx, itemB.heapIdx) = (itemB.heapIdx, itemA.heapIdx);
    }

    public bool Contains(T item)
    {
        return Equals(items[item.heapIdx], item);
    }

    public int Count => curItemCt;

    public void UpdateItem(T item)
    {
        SortUp(item);
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int heapIdx  { get; set; }
}