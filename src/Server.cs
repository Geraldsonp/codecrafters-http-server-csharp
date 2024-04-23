using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
const string port = "4221";
var ipAddress = IPAddress.Any;

try
{
    var server = new TcpListener(ipAddress, 4221);
    server.Start();

    while (true)
    {
        Console.WriteLine($"Waiting for connection on {ipAddress.ToString()}:{port}...");
        var client = await server.AcceptTcpClientAsync();
        Console.WriteLine("Connected!");
        var stream = client.GetStream();

        var response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
        await stream.WriteAsync(response);
        Console.WriteLine("Sent: {0}", response.Length);
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}