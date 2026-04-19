
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIMesh : Graphic
{
    private List<Vector2> vertices = null;
    private List<Tuple<int, int, int>> triangles = null;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (vertices == null || triangles == null)
        {
            return;
        }

        vh.Clear();

        foreach (var i in vertices)
        {
            var v = UIVertex.simpleVert;
            v.position = i;
            v.color = color;
            vh.AddVert(v);
        }

        foreach (var i in triangles)
        {
            vh.AddTriangle(i.Item1, i.Item2, i.Item3);
        }
    }

    private Vector2 ToScreenSpace(Vector2 pos)
    {
        pos = RectTransformUtility.WorldToScreenPoint(null, pos);
        pos /= transform.lossyScale;
        pos.x -= Screen.width / 2 / transform.lossyScale.x;
        pos.y -= Screen.height / 2 / transform.lossyScale.y;
        return pos;
    }

    public void CreateTriangle(RectTransform p1, RectTransform p2, RectTransform p3)
    {
        vertices = new List<Vector2>()
        {
            p1.position,
            p2.position,
            p3.position,
        };
        for (var i = 0; i < vertices.Count; i++)
        {
            vertices[i] = ToScreenSpace(vertices[i]);
        }

        triangles = new List<Tuple<int, int, int>>()
        {
            new Tuple<int, int, int>(0,1,2),
        };

        SetVerticesDirty();
    }

    public void CreateQuad(RectTransform p1, RectTransform p2, RectTransform p3, RectTransform p4)
    {
        vertices = new List<Vector2>()
        {
            p1.position,
            p2.position,
            p3.position,
            p4.position,
        };
        for (var i = 0; i < vertices.Count; i++)
        {
            vertices[i] = ToScreenSpace(vertices[i]);
        }

        triangles = new List<Tuple<int, int, int>>()
        {
            new Tuple<int, int, int>(0,1,2),
            new Tuple<int, int, int>(0,2,3),
        };

        SetVerticesDirty();
    }
}