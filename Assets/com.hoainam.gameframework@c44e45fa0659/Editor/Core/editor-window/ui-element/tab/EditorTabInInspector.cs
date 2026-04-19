
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorTabInInspector<T> : EditorBaseTab where T : Object
{
    public abstract class TabItem<T1> : TabItem where T1 : Object
    {
        public SerializedObject serializedObject;
        public T1 targetObject;
        public Editor inspector;
    }

    public EditorTabInInspector(List<TabItem> listTabItems, SerializedObject serializedObject, Editor inspector)
        :base(listTabItems)
    {
        foreach (var i in listTabItems)
        {
            var item = (TabItem<T>)i;
            item.serializedObject = serializedObject;
            item.targetObject = (T)serializedObject.targetObject;
            item.inspector = inspector;
        }
    }
}
