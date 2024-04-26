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
        HttpResponse? response;
        string body = "";

        if (httpRequest.Path.StartsWith("echo/"))
        {
            if (!string.IsNullOrEmpty(httpRequest.Path))
            {
                body = httpRequest.Path.Substring("echo/".Length);
            }

            response = HttpResponse.Ok(body);
            stream.Write(response.SerializeResponse());
        }

        if (string.IsNullOrEmpty(httpRequest.Path) || httpRequest.Path.StartsWith("user-agent"))
        {
            body = "";

            if (httpRequest.Headers.ContainsKey("User-Agent"))
            {
                body += httpRequest.Headers["User-Agent"];
            }

            response = HttpResponse.Ok(body);
            stream.Write(response.SerializeResponse());
        }


        response = HttpResponse.NotFound();
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