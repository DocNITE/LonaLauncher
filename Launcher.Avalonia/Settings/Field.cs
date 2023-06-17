namespace Launcher.Avalonia.Settings;

public class Field
{
    private string _name;
    private string _data;
    
    public string Name
    {
        get => _name;
        set { _name = value; }
    }
    
    public string Data
    {
        get => _data;
        set { _data = value; }
    }

    public Field(string name, string data)
    {
        _name = name;
        _data = data;
        
        Config.AddField(this);
    }
}