using Launcher.Client.Settings;

namespace Launcher.Client.Game;

public partial class BecapeManager
{
    public List<string> GetFiles()
    {
        var list = new List<string>();
        
        // Check Save01.rvdata2 - Save40.rvdata2
        for (var i = 1; i <= 40; i++)
        {
            var fileName = Config.GetField("game_path") + (i < 10 ? $"/Save0{i}.rvdata2" : $"/Save{i}.rvdata2");
            if (File.Exists(fileName))
                list.Add(fileName);
        }
        
        // Check SavAuto.rvdata2
        var savauto = Config.GetField("game_path") + "SavAuto.rvdata2";
        if (File.Exists(savauto))
            list.Add(savauto);
        
        // Check SavDoomMode.rvdata2
        var savdoommode = Config.GetField("game_path") + "SavDoomMode.rvdata2";
        if (File.Exists(savdoommode))
            list.Add(savdoommode);
        
        // Check GameLona.ini
        var gamelonaini = Config.GetField("game_path") + "GameLona.ini";
        if (File.Exists(gamelonaini))
            list.Add(gamelonaini);
        
        return list;
    }
}