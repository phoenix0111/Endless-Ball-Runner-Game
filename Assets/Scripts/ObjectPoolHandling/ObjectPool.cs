using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool<T>
{
    private Queue<T> queue;
    private int maxCount;

    private Func<T> OnCreate;
    private Action<T> OnGet;
    private Action<T> OnReturnToPool;
    private Action<T> OnDestroy;

    public ObjectPool(Func<T> OnCreate, Action<T> OnGet, Action<T> OnReturnToPool, Action<T> OnDestroy, int maxCount)
    {
        queue = new Queue<T>();
        this.maxCount = maxCount;

        this.OnCreate = OnCreate;
        this.OnGet = OnGet;
        this.OnReturnToPool = OnReturnToPool;
        this.OnDestroy = OnDestroy;

        for (int i = 1; i <= maxCount; i++)
        {
            T value = OnCreate.Invoke();
            queue.Enqueue(value);
        }
    }

    public T Get()
    {
        if (queue.Count == 0)
        {
            Debug.LogError("Pool is empty, returning default");
            return default(T);
        }
        T value = queue.Dequeue();
        OnGet?.Invoke(value);
        return value;
    }


    public bool ReturnToPool(T value)
    {
        if (queue.Count == maxCount)
        {
            Debug.LogError("Pool is full, can't take anymore.");
            return false;
        }
        OnReturnToPool?.Invoke(value);
        queue.Enqueue(value);
        return true;
    }

    public void Clear()
    {
        while (queue.Count > 0)
        {
            T value = queue.Dequeue();
            OnDestroy?.Invoke(value);
        }
    }
}
