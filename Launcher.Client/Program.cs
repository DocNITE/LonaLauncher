using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Loader;
using Launcher.Client;

async void OnCommand(string buffer)
{
    var param = buffer.Split(' ');

    switch (param[0])
    {
        case "download":
            // create cache
            var dirCache = new DirectoryInfo("./.cache");
            if (dirCache.Exists)
                dirCache.Delete(true);
            dirCache.Create();
            var http = new HttpDownload(param[1], "./.cache/" + param[2]);
            var canExit = false;
            http.ProgressChanged += (long? totalFileSize, long totalBytesDownloaded, double? progressPercentage) =>
            {
                Console.WriteLine("Download: " + (totalFileSize.ToString() ?? string.Empty) + ", " + totalBytesDownloaded.ToString() +
                                  ", " + progressPercentage.ToString());
                
            };
            await http.StartDownload();
            Console.WriteLine("Finished!");
            Console.WriteLine("Start process...");
            Progress<ZipProgress> _dprogress;
            _dprogress = new Progress<ZipProgress>();
            _dprogress.ProgressChanged += (object sender, ZipProgress zipProgress) =>
            {
                Console.WriteLine("Extracting: " + zipProgress.Total + ", " + zipProgress.Processed);
            };
            using (FileStream zipToOpen = new FileStream("./.cache/"+param[2], FileMode.Open))
            {
                ZipArchive zipp = new ZipArchive(zipToOpen);
                zipp.ExtractToDirectory("./Game", _dprogress);
            }
            dirCache.Delete(true);
            break;
        case "mkdir":
            var path = param[1];
            var dir = new DirectoryInfo(path);
            dir.Create();
            Console.WriteLine("Make dir: " + path);
            break;
        case "download2":
            Console.WriteLine("Start process...");
            Progress<ZipProgress> _progress;
            _progress = new Progress<ZipProgress>();
            _progress.ProgressChanged += (object sender, ZipProgress zipProgress) =>
            {
                Console.WriteLine(zipProgress.Total + ", " + zipProgress.Processed);
            };
            WebClient wc = new WebClient();
            Stream zipReadingStream = wc.OpenRead(param[1]);
            ZipArchive zip = new ZipArchive(zipReadingStream);
            zip.ExtractToDirectory(param[2], _progress);
            break;
    }
}

while (true)
{
    var param = Console.ReadLine();
    if (param == null) continue;
    if (param == "exit") break;
    
    OnCommand(param);
}