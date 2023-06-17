using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Launcher.Avalonia;

public class HttpDownload : IDisposable
{
    private readonly string _downloadUrl;
    private readonly string _destinationFilePath;

    private HttpClient _httpClient;

    public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

    public event ProgressChangedHandler ProgressChanged;

    public HttpDownload(string downloadUrl, string destinationFilePath)
    {
        _downloadUrl = downloadUrl;
        _destinationFilePath = destinationFilePath;
    }

    public async Task StartDownload()
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };

        using (var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            await DownloadFileFromHttpResponseMessage(response);
    }

    private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;

        using (var contentStream = await response.Content.ReadAsStreamAsync())
            await ProcessContentStream(totalBytes, contentStream);
    }

    private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
    {
        var totalBytesRead = 0L;
        var readCount = 0L;
        var buffer = new byte[8192];
        var isMoreToRead = true;

        using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
        {
            do
            {
                var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    isMoreToRead = false;
                    TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    continue;
                }

                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalBytesRead += bytesRead;
                readCount += 1;

                if (readCount % 100 == 0)
                    TriggerProgressChanged(totalDownloadSize, totalBytesRead);
            }
            while (isMoreToRead);
        }
    }

    private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
    {
        if (ProgressChanged == null)
            return;

        double? progressPercentage = null;
        if (totalDownloadSize.HasValue)
            progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

        ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

//This is a new class that represents a progress object.
public class ZipProgress
{
    public ZipProgress(int total, int processed, string currentItem)
    {
        Total = total;
        Processed = processed;
        CurrentItem = currentItem;
    }
    public int Total { get; }
    public int Processed { get; }
    public string CurrentItem { get; }
}

public static class MyZipFileExtensions
{
    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<ZipProgress> progress)
    {
        ExtractToDirectory(source, destinationDirectoryName, progress, overwrite: true);
    }

    public static void ExtractToDirectory(this ZipArchive source, string destinationDirectoryName, IProgress<ZipProgress> progress, bool overwrite)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (destinationDirectoryName == null)
            throw new ArgumentNullException(nameof(destinationDirectoryName));


        // Rely on Directory.CreateDirectory for validation of destinationDirectoryName.

        // Note that this will give us a good DirectoryInfo even if destinationDirectoryName exists:
        DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
        string destinationDirectoryFullPath = di.FullName;

        int count = 0;
        foreach (ZipArchiveEntry entry in source.Entries)
        {
            count++;
            string fileDestinationPath = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, entry.FullName));

            if (!fileDestinationPath.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                throw new IOException("File is extracting to outside of the folder specified.");

            var zipProgress = new ZipProgress(source.Entries.Count, count, entry.FullName);
            progress.Report(zipProgress);

            if (Path.GetFileName(fileDestinationPath).Length == 0)
            {
                // If it is a directory:

                if (entry.Length != 0)
                    throw new IOException("Directory entry with data.");

                Directory.CreateDirectory(fileDestinationPath);
            }
            else
            {
                // If it is a file:
                // Create containing directory:
                Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath));
                entry.ExtractToFile(fileDestinationPath, overwrite: overwrite);
            }
        }
    }
}