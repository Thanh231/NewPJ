using System.Collections;
using UnityEngine;

public class FlashObject : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float flashSpeed = 3f;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 0.8f;
    [SerializeField] private float flashDuration = 0.5f;

    private Renderer objectRenderer;
    private Material objectMaterial;

    private bool isFlashing = false;
    public GameObject test;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        objectRenderer = GetComponent<MeshRenderer>();
        objectMaterial = objectRenderer.material;
    }

    private void OnEnable()
    {
        EventManager.OnStartGame += ResetFlash;
        EventManager.OnQueueFull += StartFlashing;
        EventManager.OnQueueNotFull += StopFlashing;
    }

    private void OnDisable()
    {
        EventManager.OnStartGame -= ResetFlash;
        EventManager.OnQueueFull -= StartFlashing;
        EventManager.OnQueueNotFull -= StopFlashing;
    }

    private void ResetFlash()
    {
        StopFlashing();
        if (objectMaterial != null)
        {
            Color currentColor = objectMaterial.GetColor("_BaseColor");
            currentColor.a = 0f;
            objectMaterial.SetColor("_BaseColor", currentColor);
        }
    }

    private void StartFlashing()
    {
        test.SetActive(true);
        if (!isFlashing)
        {
            isFlashing = true;

            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }

            flashCoroutine = StartCoroutine(FlashCoroutine());
        }
    }

    private void StopFlashing()
    {
        test.SetActive(false);
        isFlashing = false;
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        // if (objectRenderer != null)
        // {
        //     Color currentColor = objectMaterial.GetColor("_BaseColor");
        //     currentColor.a = 0f;
        //     objectMaterial.SetColor("_BaseColor", currentColor);
        // }
    }

    private IEnumerator FlashCoroutine()
    {
        float totalElapsed = 0f;

        while (isFlashing && totalElapsed < flashDuration)
        {
            totalElapsed += Time.deltaTime;
            yield return null;
        }

        StopFlashing();
    }
}

