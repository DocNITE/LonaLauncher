namespace Launcher.LoaderAPI;

public class DownloadProgress
{
    public DownloadProgress(long total, long processed, double progress)
    {
        TotalSize = total;
        ProcessedBytes = processed;
        Progress = progress;
    }
    public long TotalSize { get; }
    public long ProcessedBytes { get; }
    public double Progress { get; }
}

public class HttpDownloader : IDisposable
{
    private readonly string _downloadUrl;
    private readonly string _destinationFilePath;

    public HttpClient NetClient;

    public delegate void ProgressChangedHandler(DownloadProgress progress);

    public event ProgressChangedHandler ProgressChanged;

    public HttpDownloader(string downloadUrl, string destinationFilePath)
    {
        NetClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };
        
        _downloadUrl = downloadUrl;
        _destinationFilePath = destinationFilePath;
    }

    public async Task StartDownload()
    {
        using (var response = await NetClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
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

        var downloadProgress = new DownloadProgress(totalDownloadSize ?? 0, totalBytesRead, progressPercentage ?? 0);
        ProgressChanged(downloadProgress);
    }

    public void Dispose()
    {
        NetClient?.Dispose();
    }
}