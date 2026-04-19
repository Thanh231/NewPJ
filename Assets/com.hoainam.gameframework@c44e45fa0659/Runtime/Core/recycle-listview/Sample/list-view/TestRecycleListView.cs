
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestRecycleListView : MonoBehaviour
{
    public RecycleListView recycleListView;

    private void Start()
    {
        var l = new List<object>();
        for (int i = 0; i < 100; i++)
        {
            l.Add(i);
        }
        recycleListView.SetData(l);
    }
}