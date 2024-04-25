using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_http_server;

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
        var httpRequest = await ReadIncoming(stream);

        var slashIndex = httpRequest.Path.IndexOf("/", 1);
        var stringToEcho = httpRequest.Path.Substring(slashIndex + 1);
        var response = HttpResponse.Ok(stringToEcho);
        stream.Write(response.SerializeResponse());
    }

    async Task<HttpRequest> ReadIncoming(Stream stream)
    {
        var buffer = new byte[1024];
        var bytesRead = await stream.ReadAsync(buffer);
        var data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received: {0}", data);

        return HttpRequest.Parse(data);
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}