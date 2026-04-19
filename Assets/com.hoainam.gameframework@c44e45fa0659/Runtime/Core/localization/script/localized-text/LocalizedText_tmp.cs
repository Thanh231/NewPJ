
using TMPro;
using UnityEngine;

[ExecuteAlways]
public partial class LocalizedText_tmp : LocalizedText
{
    public Color underlayColor = Color.black;
    
    private TMP_Text _tmp;
    public TMP_Text tmp
    {
        get
        {
            if (_tmp == null)
            {
                _tmp = GetComponent<TMP_Text>();
            }
            return _tmp;
        }
    }

    public override void SetText(string text)
    {
        tmp.text = text;
    }

    protected override void Start()
    {
        base.Start();
        
        if (Application.IsPlaying(gameObject))
        {
            StartRuntime();
        }
        else
        {
            StartEditor();
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (!Application.IsPlaying(gameObject))
        {
            UpdateEditor();
        }
    }
}
