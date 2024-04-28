using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_http_server;

// You can use print statements as follows for debugging, they'll be visible when running tests.

internal class Program
{
    private static string rootFolder;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Logs from your program will appear here!");
        rootFolder = args.Length > 0 ? args[1] : "files";
        const string port = "4221";
        var ipAddress = IPAddress.Any;
        try
        {
            var server = new TcpListener(ipAddress, 4221);
            server.Start();

            while (true)
            {
                Console.WriteLine($"Waiting for connection on {ipAddress.ToString()}:{port}...");
                Console.WriteLine($"Serving files from {rootFolder}");

                var client = await server.AcceptTcpClientAsync();
                _ = HandleConnection(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task HandleConnection(TcpClient client)
    {
        Console.WriteLine("Connected!");
        var connectionStream = client.GetStream();
        var request = await ReadIncoming(connectionStream);
        HttpResponse? response;

        switch (request.Method)
        {
            case "GET":
                Console.WriteLine("Handling GET request...");
                response = await HandleGet(request);
                break;
            case "POST":
                Console.WriteLine("Handling POST request...");
                response = await HandlePost(request);
                break;
            default:
                Console.WriteLine("Handling unsupported request...");
                response = HttpResponse.BadRequest();
                break;
        }
        
        connectionStream.Write(response.SerializeResponse());
    }

    private static async Task<HttpResponse> HandlePost(HttpRequest request)
    {
        var filePath = Path.Combine(rootFolder, request.Path.Substring("files/".Length));
        await File.WriteAllTextAsync(filePath, request.Body);
        var response = HttpResponse.Created("File created");
        response.AddHeader("Content-Type", "text/plain");
        return response;
    }

    private static async Task<HttpResponse> HandleGet(HttpRequest request)
    {
        var body = "";
        HttpResponse response;
        if (request.Path.StartsWith("files/"))
        {
            var filePath = request.Path.Substring("files/".Length);
            var contentType = "application/octet-stream";
            var fullPath = Path.Combine(rootFolder, filePath);
            if (File.Exists(fullPath))
            {
                body = File.ReadAllText(fullPath);
                response = HttpResponse.Ok(body);
                response.Headers["Content-Type"] = contentType;
                return response;
            }
            else
            {
                response = HttpResponse.NotFound();
                return response;
            }
        }

        if (request.Path.StartsWith("echo/"))
        {
            if (!string.IsNullOrEmpty(request.Path))
            {
                body = request.Path.Substring("echo/".Length);
            }

            response = HttpResponse.Ok(body);
            return response;
        }

        if (string.IsNullOrEmpty(request.Path) || request.Path.StartsWith("user-agent"))
        {
            body = "";

            if (request.Headers.ContainsKey("User-Agent"))
            {
                body += request.Headers["User-Agent"];
            }

            response = HttpResponse.Ok(body);
            return response;
        }


        response = HttpResponse.NotFound();
        return response;
    }


    static async Task<HttpRequest> ReadIncoming(Stream stream)
    {
        var buffer = new byte[1024];
        var bytesRead = await stream.ReadAsync(buffer);
        var data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received: {0}", data);

        return HttpRequest.Parse(data);
    }
}