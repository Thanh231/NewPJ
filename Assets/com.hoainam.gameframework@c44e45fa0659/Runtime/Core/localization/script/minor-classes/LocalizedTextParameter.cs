


//localized text = text with param: {0}
//if {0} = int, float, string, .... => pass primitive to list params
//if {0} = other localized key, pass this object to list params
public class LocalizedTextParameter
{
    public string key;
    public object[] parameters;

    public LocalizedTextParameter(string key)
    {
        this.key = key;
    }

    public LocalizedTextParameter(string key, object[] parameters)
    {
        this.key = key;
        this.parameters = parameters;
    }

    public override string ToString()
    {
        return LocalizationController.instance.LocalizedTextParameterToString(this);
    }
}