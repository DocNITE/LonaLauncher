using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Launcher.Avalonia.Settings;

public sealed class Config
{
    public const string SavePath = ".data/";
    public const string FileName = "settings.txt";

    private const char _seperator = '=';
    private const char _nextline = '\n';
    private static List<Field> _fields = new List<Field>();

    public static void AddField(Field field)
    {
        _fields.Add(field);
    }
    
    public static string? GetField(string name)
    {
        return (from field in _fields where field.Name == name select field.Data).FirstOrDefault();
    }

    public static void Load()
    {
        var dir = new DirectoryInfo(SavePath);

        if (!dir.Exists)
        {
            Model(); // Generate new settings
            Save();
            return;
        }
        
        var fs = File.Open(SavePath + FileName, FileMode.Open);
        var sr = new StreamReader(fs);

        while (true)
        {
            var field = sr.ReadLine();
            if (field == null) break;
            
            var values = field.Split(_seperator);
            var fieldObj = new Field(values[0], values[1]);
        }
    }
    
    public static void Save()
    {
        var dir = new DirectoryInfo(SavePath);
        
        if (dir.Exists)
            dir.Delete(true);
        
        dir.Create();
        var fs = File.Create(SavePath + FileName);
        var sr = new StreamWriter(fs);
        
        if (File.Exists(SavePath + FileName))
        {
            Console.WriteLine("File settings is created.");
            foreach (var field in _fields)
            {
                Console.WriteLine("Written field: " + field.Name);
                sr.WriteLine(field.Name + _seperator + field.Data);
            }
            sr.Close();
            fs.Close();
        }
        else
        {
            Console.WriteLine("File settings is not created.");
        }
    }

    public static void Model()
    {
        var fields =
        new[] {
            new Field("game_version", "-1"),
            new Field("launcher_version", "-1"),
            new Field("game_path", "./Game"),
            new Field("game_url", "https://github.com/DocNITE/LonaData/releases/download/0.8.1.1/Game.zip"),
            new Field("patch_url", "https://github.com/DocNITE/LonaData/releases/download/0.8.1.1/Patch.zip"),
            new Field("game_version_url", "https://github.com/DocNITE/LonaData/releases/download/0.8.1.1/Game-Version.txt"),
            new Field("launcher_version_url", "https://github.com/DocNITE/LonaData/releases/download/0.8.1.1/Launcher-Version.txt")
        };
    }
}