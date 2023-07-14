namespace Launcher.Client.Settings;

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
        set 
        { 
            Environment.SetEnvironmentVariable(Config.GetEnvironmentName(Name), 
                                            value, 
                                            EnvironmentVariableTarget.User);
            _data = value;
        }
    }

    public Field(string name, string data, bool restored = false)
    {
        Name = name;
        
        var value = Environment.GetEnvironmentVariable(Config.GetEnvironmentName(Name), 
                                                    EnvironmentVariableTarget.User);
        if (value != null)
        {
            if (!restored)
                Data = value;
            else 
                Data = data;
        } 
        else 
        {
            Data = data;
        }

        Config.AddField(this);
    }
}