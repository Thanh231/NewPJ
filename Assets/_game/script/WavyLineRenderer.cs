using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavyLineRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [Header("Wave Settings")]
    public int waveSegments;
    public float waveAmplitudeMin;
    public float waveAmplitudeMax;
    public float amplitudeChangeSpeed;
    public float waveFrequency;
    public float waveSpeed;
    private float _waveTime;
    private float _amplitudeTime;
    private float _currentAmplitude = 0.03f;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _targetEndPoint;
    private Color _baseColor = Color.yellow;

    [Header("Target Management")]
    [SerializeField] private float baseSingleTargetDuration;

    private List<GameObject> _targetBlocks = new List<GameObject>();
    private Coroutine _targetProcessCoroutine;
    private GameObject _currentTarget;
    private PigComponent _pigComponent;
    private System.Action _onBulletChanged;

    public Material lineMaterial;
    private float _speedMultiplier = 1f;

    private void Awake()
    {
        InitializeLineRenderer();
        _pigComponent = GetComponent<PigComponent>();
    }

    private void OnDestroy()
    {
        ClearAllTargets();
    }

    public void SetBulletChangedCallback(System.Action callback)
    {
        _onBulletChanged = callback;
    }

    public void InitializeLineRenderer(float startWidth = 0.06f, float endWidth = 0.06f, float amplitude = 0.005f)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            return;
        }

        _lineRenderer.startWidth = startWidth;
        _lineRenderer.endWidth = endWidth;
        _lineRenderer.positionCount = waveSegments;
        _currentAmplitude = amplitude;

        _lineRenderer.material = lineMaterial;

        _lineRenderer.startColor = Color.yellow;
        _lineRenderer.endColor = Color.yellow;
        _lineRenderer.enabled = false;
        _lineRenderer.sortingOrder = 100;
        _lineRenderer.useWorldSpace = true;
    }

    public void SetColor(Color color)
    {
        _baseColor = color;

        _lineRenderer.material = lineMaterial;
        _lineRenderer.material.color = _baseColor;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    public void AddTarget(GameObject block)
    {
        _targetBlocks.Add(block);

        if (_targetProcessCoroutine == null)
        {
            _targetProcessCoroutine = StartCoroutine(ProcessTargets());
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.01f, multiplier);  // tránh chia cho 0
                                                          // waveSpeed = waveSpeed * multiplier;  // mở nếu muốn sóng rung nhanh hơn
    }

    private float ComputePerTargetDuration()
    {
        int count = Mathf.Max(1, _targetBlocks.Count);
        float totalDuration = baseSingleTargetDuration;
        float perTarget = totalDuration / count / _speedMultiplier;
        return Mathf.Max(0.03f, perTarget);
    }

    private IEnumerator ProcessTargets()
    {
        while (_targetBlocks.Count > 0)
        {
            _currentTarget = _targetBlocks[0];

            // Bỏ qua target null
            if (_currentTarget == null)
            {
                _targetBlocks.RemoveAt(0);
                continue;
            }

            _targetEndPoint = _currentTarget.transform.position;
            _endPoint = _targetEndPoint;

            // === Setup line renderer ===
            float distance = Vector3.Distance(_startPoint, _endPoint);
            _lineRenderer.material.mainTextureScale = new Vector2(1f, distance * 2f);
            _lineRenderer.enabled = true;

            AudioGameManger.instance.PlaySFX(AudioIndex.yarn);
            HapticController.PlayHaptic(HapticType.collect_yarn);

            // === Vòng lặp animation sóng đứng (tính duration động) ===
            float elapsed = 0f;
            float perTargetDuration = ComputePerTargetDuration();

            while (elapsed < perTargetDuration)
            {
                if (_currentTarget == null)
                    break;

                // Cập nhật duration mỗi frame — khi raycast add thêm target thì co lại
                perTargetDuration = ComputePerTargetDuration();

                _targetEndPoint = _currentTarget.transform.position;
                _endPoint = _targetEndPoint;

                _waveTime += Time.deltaTime * waveSpeed;
                _amplitudeTime += Time.deltaTime * amplitudeChangeSpeed;
                _currentAmplitude = Mathf.Lerp(waveAmplitudeMin, waveAmplitudeMax,
                    (Mathf.Sin(_amplitudeTime) + 1f) * 0.5f);

                // Trục vuông góc với dây
                Vector3 waveDirection = _endPoint - _startPoint;
                Vector3 waveRight = Vector3.Cross(waveDirection.normalized, Vector3.up).normalized;
                if (waveRight == Vector3.zero)
                    waveRight = Vector3.Cross(waveDirection.normalized, Vector3.forward).normalized;

                // Vẽ sóng đứng (2 đầu cố định)
                for (int i = 0; i < waveSegments; i++)
                {
                    float t = i / (float)(waveSegments - 1);
                    Vector3 point = Vector3.Lerp(_startPoint, _endPoint, t);

                    float spatialWave = Mathf.Sin(t * waveFrequency * Mathf.PI);
                    float temporalWave = Mathf.Sin(_waveTime);
                    float wave = spatialWave * temporalWave * _currentAmplitude;

                    point += waveRight * wave;
                    _lineRenderer.SetPosition(i, point);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // === Destroy target hiện tại ===
            if (_currentTarget != null)
            {
                _currentTarget.GetComponent<Block>().StartDestroy();
                EventManager.OnBlockDestroyed?.Invoke();

                if (_pigComponent != null)
                {
                    _pigComponent.Bullet--;
                    if (_pigComponent.bulletText != null)
                        _pigComponent.bulletText.text = _pigComponent.Bullet.ToString();

                    _onBulletChanged?.Invoke();
                }
            }

            if (_targetBlocks.Count > 0)
                _targetBlocks.RemoveAt(0);
        }

        // === Cleanup ===
        _lineRenderer.enabled = false;
        _targetProcessCoroutine = null;
        _currentTarget = null;
    }
    public void ClearAllTargets()
    {
        if (_targetProcessCoroutine != null)
        {
            StopCoroutine(_targetProcessCoroutine);
            _targetProcessCoroutine = null;
        }

        foreach (GameObject go in _targetBlocks)
        {
            if (go != null)
            {
                var block = go.GetComponent<Block>();
                block.isAlreadyDestroyed = false;
                block.SetUnactive(true);
            }
        }
        _targetBlocks.Clear();
        _currentTarget = null;
        _lineRenderer.enabled = false;
    }


    public void UpdateStartPoint(Vector3 startPoint)
    {
        _startPoint = new Vector3(startPoint.x, startPoint.y, startPoint.z);
    }

    public void HideLineImmediately()
    {
        _lineRenderer.enabled = false;
    }

    public bool IsProcessing => _targetProcessCoroutine != null;

    public Vector3? GetCurrentTargetPosition()
    {
        if (_currentTarget != null)
        {
            return _currentTarget.transform.position;
        }
        return null;
    }
}



