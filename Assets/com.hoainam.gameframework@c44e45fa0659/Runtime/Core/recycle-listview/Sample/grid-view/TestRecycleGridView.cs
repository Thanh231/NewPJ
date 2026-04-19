using System.Collections.Generic;
using UnityEngine;

public class TestRecycleGridView : MonoBehaviour
{
    public RecycleGridView recycleGridView;

    private void Start()
    {
        var l = new List<object>();
        for (int i = 0; i < 100; i++)
        {
            l.Add(i);
        }
        recycleGridView.SetData(l);
    }
}
