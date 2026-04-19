using UnityEngine;

public class WorldSpaceCanvasScaler : MonoBehaviour
{
	private Camera mainCamera;
	private Vector2Int lastResolution;
	private RectTransform canvasRectTransform;

	private void Start()
	{
		mainCamera = Camera.main;
		canvasRectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (lastResolution.x != Screen.width || lastResolution.y != Screen.height)
		{
			var h = 2 * mainCamera.orthographicSize;
			var w = h * ((float)Screen.width / Screen.height);

			canvasRectTransform.anchoredPosition = new Vector2(0, 0);
			canvasRectTransform.sizeDelta = new Vector2(w, h);

			lastResolution = new Vector2Int(Screen.width, Screen.height);
		}
	}
}