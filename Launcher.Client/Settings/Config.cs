using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Launcher.Client.Settings;

public sealed class Config
{
    private const string ConfigFieldIdx = "LONA_LAUNCHER_CFG";
    private const string ConfigVersion = "1";
    private const string ConfigVersionIdx = "VERSION";

    private static readonly List<Field> _fields = new();

    public static void AddField(Field field)
    {
        _fields.Add(field);
    }
    
    public static string? GetField(string name)
    {
        return (from field in _fields where field.Name == name select field.Data).FirstOrDefault();
    }
    
    public static Field? GetFieldObject(string name)
    {
        return (from field in _fields where field.Name == name select field).FirstOrDefault();
    }
    
    public static Field? GetFieldObjectWithVersion(string groupName)
    {
        return (from item in _fields let nameSp = item.Name.Split("_") where nameSp.Length == 2 && nameSp[0] == groupName && nameSp[1] == "version" select item).FirstOrDefault();
    }

    public static string GetEnvironmentName(string fieldName) => ConfigFieldIdx + "_" + fieldName;

    public static void Load()
    {
        var restoreData = true;
        /* For now no need to config check. TODO: Need to make config update
        var version = Environment.GetEnvironmentVariable(GetEnvironmentName(ConfigVersionIdx), EnvironmentVariableTarget.User);
        var restoreData = false;

        if (version != null)
        {
            if (version != ConfigVersion)
            {
                restoreData = true;
                Environment.SetEnvironmentVariable(GetEnvironmentName(ConfigVersionIdx), ConfigVersion, EnvironmentVariableTarget.User);
            }
        } 
        else 
        {
            restoreData = true;
            Environment.SetEnvironmentVariable(GetEnvironmentName(ConfigVersionIdx), ConfigVersion, EnvironmentVariableTarget.User);
        }
        */

        // WARNING: MONKEY CODE
        // Yeah, in better way - we should use some generic spaces (not hardcode).
        var link = "https://github.com/DocNITE/LonaData/releases/download/0.8.1.1/";

        _ =
        new[] {
            new Field("game_version", "-1", restoreData),
            new Field("patch_version", "-1", restoreData),
            new Field("game_path", "./Game", restoreData),
            new Field("game_url", link + "Game.zip", restoreData),
            new Field("patch_url", link + "Patch.zip", restoreData),
            new Field("patch_version_url", link + "Patch-Version.txt", restoreData),
            new Field("game_version_url", link + "Game-Version.txt", restoreData),
        };
    }
}