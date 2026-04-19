using UnityEngine;

public abstract class LocalizedText : MonoBehaviour
{
	public string key;
	public object[] parameters { get; set; }
	
	public abstract void SetText(string text);
	
	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}
	
	public void SetKey(string key)
	{
		this.key = key;
		LocalizationController.instance.SetupLocalizedText(this);
	}

	public void SetParameters(object[] parameters)
	{
		this.parameters = parameters;
		LocalizationController.instance.SetupLocalizedText(this);
	}

	public void SetKeyAndParameters(string key, object[] parameters)
	{
		this.key = key;
		this.parameters = parameters;
		LocalizationController.instance.SetupLocalizedText(this);
	}
}