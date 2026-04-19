using System;
using System.Collections.Generic;
using ObservableCollections;
using UnityEngine;

public static partial class StaticUtils
{
    #region list

    public static List<List<T>> Split<T>(this List<T> l, int sz)
    {
        var result = new List<List<T>>();
        foreach (var i in l)
        {
            if (result.Count == 0)
            {
                result.Add(new List<T>());
            }
            var lastList = result[^1];
            if (lastList.Count >= sz)
            {
                lastList = new List<T>();
                result.Add(lastList);
            }
            lastList.Add(i);
        }
        return result;
    }


    #endregion

    #region rx list

    public static T Find<T>(this ObservableList<T> l, Predicate<T> match) where T : class
    {
        foreach (var i in l)
        {
            if (match.Invoke(i))
            {
                return i;
            }
        }
        return null;
    }

    #endregion

    #region 1-D array

    public static int IndexOf<T>(this T[] arr, T item)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    public static bool Exists<T>(this T[] arr, Predicate<T> match)
    {
        foreach (var i in arr)
        {
            if (match.Invoke(i))
            {
                return true;
            }
        }
        return false;
    }

    public static T Find<T>(this T[] arr, Predicate<T> match) where T : class
    {
        foreach (var i in arr)
        {
            if (match.Invoke(i))
            {
                return i;
            }
        }
        return null;
    }

    public static List<T> FindAll<T>(this T[] arr, Predicate<T> match)
    {
        var result = new List<T>();
        foreach (var i in arr)
        {
            if (match.Invoke(i))
            {
                result.Add(i);
            }
        }

        return result;
    }

    public static T? FindStruct<T>(this T[] arr, Predicate<T> match) where T : struct
    {
        foreach (var i in arr)
        {
            if (match.Invoke(i))
            {
                return i;
            }
        }
        return null;
    }

    #endregion

    #region 2-D array

    public static Vector2Int? IndexOf<T>(this T[,] arr, T item)
    {
        var rows = arr.GetLength(0);
        var cols = arr.GetLength(1);
        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
            {
                if (item.Equals(arr[y, x]))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    #endregion
}