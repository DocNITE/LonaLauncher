/*
using System.Net.Sockets;
 
string host = "127.0.0.1";
int port = 8888;
using TcpClient client = new TcpClient();
Console.Write("Введите свое имя: ");
string? userName = Console.ReadLine();
Console.WriteLine($"Добро пожаловать, {userName}");
StreamReader? Reader = null;
StreamWriter? Writer = null;
 
try
{
    client.Connect(host, port); //подключение клиента
    Reader = new StreamReader(client.GetStream());
    Writer = new StreamWriter(client.GetStream());
    if (Writer is null || Reader is null) return;
    // запускаем новый поток для получения данных
    Task.Run(()=>ReceiveMessageAsync(Reader));
    // запускаем ввод сообщений
    await SendMessageAsync(Writer);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Writer?.Close();
Reader?.Close();
 
// отправка сообщений
async Task SendMessageAsync(StreamWriter writer)
{
    // сначала отправляем имя
    await writer.WriteLineAsync(userName);
    await writer.FlushAsync();
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
 
    while (true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}
// получение сообщений
async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            // считываем ответ в виде строки
            string? message = await reader.ReadLineAsync();
            // если пустой ответ, ничего не выводим на консоль
            if (string.IsNullOrEmpty(message)) continue; 
            Print(message);//вывод сообщения
        }
        catch
        {
            break;
        }
    }
}
// чтобы полученное сообщение не накладывалось на ввод нового сообщения
void Print(string message)
{
    if (OperatingSystem.IsWindows())    // если ОС Windows
    {
        var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
        int left = position.Left;   // смещение в символах относительно левого края
        int top = position.Top;     // смещение в строках относительно верха
        // копируем ранее введенные символы в строке на следующую строку
        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
        // устанавливаем курсор в начало текущей строки
        Console.SetCursorPosition(0, top);
        // в текущей строке выводит полученное сообщение
        Console.WriteLine(message);
        // переносим курсор на следующую строку
        // и пользователь продолжает ввод уже на следующей строке
        Console.SetCursorPosition(left, top + 1);
    }
    else Console.WriteLine(message);
}
TCP EXAMPLE */

/*
using System.Net;
using System.Net.Sockets;
using System.Text;
 
IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Введите свое имя: ");
string? username = Console.ReadLine();
Console.Write("Введите порт для приема сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Введите порт для отправки сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();
 
// запускаем получение сообщений
Task.Run(ReceiveMessageAsync);
// запускаем ввод и отправку сообщений
await SendMessageAsync();
 
// отправка сообщений в группу
async Task SendMessageAsync()
{
    using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
    // отправляем сообщения
    while (true)
    {
        var message = Console.ReadLine(); // сообщение для отправки
        // если введена пустая строка, выходим из цикла и завершаем ввод сообщений
        if (string.IsNullOrWhiteSpace(message)) break;
        // иначе добавляем к сообщению имя пользователя
        message = $"{username}: {message}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        // и отправляем на 127.0.0.1:remotePort
        await sender.SendToAsync(data, new IPEndPoint(localAddress, remotePort));
    }
}
// отправка сообщений
async Task ReceiveMessageAsync()
{
    byte[] data = new byte[65535]; // буфер для получаемых данных
    // сокет для прослушки сообщений
    using Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    // запускаем получение сообщений по адресу 127.0.0.1:localPort
    receiver.Bind(new IPEndPoint(localAddress, localPort));
    while (true)
    {
        // получаем данные в массив data
        var result = await receiver.ReceiveFromAsync(data, new IPEndPoint(IPAddress.Any, 0));
        var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
        // выводим сообщение
        Console.WriteLine(message);
    }
}
*/


// TODO - Make with httpClient