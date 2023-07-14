using System.IO.Compression;

namespace Launcher.LoaderAPI;

public class HttpManager
{
    public static Action? OnExistingFile;
    public static Action<string, string>? OnChangeFileVersion;
    public static Action<DownloadProgress>? OnDownloadProgress;
    public static Action<ZipProgress>? OnZipExtractProgress;
    public static Action? OnDownloadComplete;

    private static bool _isDownload = false;
    public static string CacheFileName = "Cache";
    public static string CacheDirectory = "./.cache";
    
    public static async Task DownloadCoreFile(string groupFiles, string url, string path, string versionUrl, string? currentVersion = "-1")
    {
        if (_isDownload)
            return;
        
        _isDownload = true;

        var dirCache = new DirectoryInfo(CacheDirectory);
        if (dirCache.Exists)
            dirCache.Delete(true);
        dirCache.Create();
        
        var http = new HttpDownloader(url, CacheDirectory + CacheFileName);
      
        var response = await http.NetClient.GetAsync(versionUrl);

        if (!response.IsSuccessStatusCode)
        {
            _isDownload = false;
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        content = content.Replace("\n", "");

        if (content == currentVersion || content == "-1")
        {
            OnExistingFile?.Invoke();
            dirCache.Delete(true);
            _isDownload = false;
            return;
        }
        
        OnChangeFileVersion?.Invoke(groupFiles, content);
        
        http.ProgressChanged += (progress) =>
        {
            OnDownloadProgress?.Invoke(progress);
        };
        await http.StartDownload();
        
        Progress<ZipProgress> progress = new();
        progress.ProgressChanged += (sender, zipProgress) =>
        {
            OnZipExtractProgress?.Invoke(zipProgress);
        };
        
        await using (var zipToOpen = new FileStream(CacheDirectory + CacheFileName, FileMode.Open))
        {
            var zip = new ZipArchive(zipToOpen);
            zip.ExtractToDirectory(path, progress);
        }
        
        dirCache.Delete(true);
        OnDownloadComplete?.Invoke();
        _isDownload = false;
    }
}