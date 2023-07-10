using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Launcher.Avalonia;
using Launcher.LoaderAPI;

var assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Launcher.Avalonia.dll");
var myType = assembly.GetType("Launcher.Avalonia.MainProgram");

// Check if the type was found
if (myType == null)
    return;

// Create an instance of the type using Activator.CreateInstance
var instance = Activator.CreateInstance(myType);

// Run application
var method = myType.GetMethod("Main");
if (method != null) method.Invoke(instance, new object[] { args });