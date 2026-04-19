using System;
using Unity.Profiling.Memory;
using UnityEngine;
using UnityEngine.UI;

public class MemorySnapshotButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        button.interactable = false;
        var now = DateTime.Now;
        var filename =
            $"{Application.productName}_{now.Year}-{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}.snap";
        var path = $"{Application.persistentDataPath}/{filename}";
        const CaptureFlags flag = CaptureFlags.ManagedObjects |
                                  CaptureFlags.NativeObjects |
                                  CaptureFlags.NativeAllocations |
                                  CaptureFlags.NativeAllocationSites |
                                  CaptureFlags.NativeStackTraces;
        MemoryProfiler.TakeSnapshot(path, OnTakeSnapshotDone, flag);
    }

    private void OnTakeSnapshotDone(string path, bool success)
    {
        button.interactable = true;
        Debug.LogError($"take snapshot done, result={success} path={path}");
    }
}
