
using System;
using System.Collections.Generic;

public class Deque<T>
{
	private LinkedList<T> linkedList = new LinkedList<T>();

	public int Count => linkedList.Count;

	public void Clear()
	{
		linkedList.Clear();
	}

	public T Dequeue_begin()
	{
		if (Count == 0)
		{
			throw new Exception("cannot dequeue from an empty queue");
		}

		var val = linkedList.First.Value;
		linkedList.RemoveFirst();
		return val;
	}

	public T Dequeue_end()
	{
		if (Count == 0)
		{
			throw new Exception("cannot dequeue from an empty queue");
		}

		var val = linkedList.Last.Value;
		linkedList.RemoveLast();
		return val;
	}

	public void Enqueue_begin(T val)
	{
		linkedList.AddFirst(val);
	}

	public void Enqueue_end(T val)
	{
		linkedList.AddLast(val);
	}

	public T Peek_begin()
	{
		if (Count == 0)
		{
			throw new Exception("cannot peek from an empty queue");
		}

		return linkedList.First.Value;
	}

	public T Peek_end()
	{
		if (Count == 0)
		{
			throw new Exception("cannot peek from an empty queue");
		}

		return linkedList.Last.Value;
	}
}