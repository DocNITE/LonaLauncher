namespace Launcher.LoaderAPI;

public partial class HttpManager
{
    public Action? OnExistingFile;
    public Action<string, string>? OnChangeFileVersion;
    public Action<DownloadProgress>? OnDownloadProgress;
    public Action<ZipProgress>? OnZipExtractProgress;
    public Action? OnDownloadComplete;

    private bool _isDownload = false;
    public string CacheFileName = "Cache";
    public string CacheDirectory = "./.cache";
    
}