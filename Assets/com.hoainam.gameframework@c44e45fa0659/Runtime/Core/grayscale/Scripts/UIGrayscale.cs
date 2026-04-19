using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIGrayscale : BaseMeshEffect
{
	[Range(0, 1)] [SerializeField] private float grayscale;
	public float Grayscale
	{
		get => grayscale;
		set
		{
			grayscale = value;
			image.SetVerticesDirty();
		}
	}

	private Image _image;
	private Image image
	{
		get
		{
			if (!_image)
			{
				_image = GetComponent<Image>();
			}
			return _image;
		}
	}

	protected override void Start()
	{
		base.Start();

		var material = Resources.Load<Material>("ui-grayscale");
		image.material = material;
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		
		var verts = new List<UIVertex>();
		vh.GetUIVertexStream(verts);

		for (var i = 0; i < verts.Count; i++)
		{
			var v= verts[i];
			v.uv3 = new Vector4(Grayscale, Grayscale, Grayscale, Grayscale);
			verts[i] = v;
		}
		
		vh.Clear();
		vh.AddUIVertexTriangleStream(verts);
	}
}
