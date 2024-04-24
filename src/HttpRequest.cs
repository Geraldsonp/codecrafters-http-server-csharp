namespace codecrafters_http_server;

public class HttpRequest
{
    public string Path { get; set; }
    public string Method { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }

    public HttpRequest(string method, string path, string body, Dictionary<string, string> headers)
    {
        Method = method;
        Path = path;
        Body = body;
        Headers = headers;
    }

    public static HttpRequest Parse(string request)
    {
        var lines = request.Split("\r\n");
        var firstLine = lines[0].Split(" ");
        var method = firstLine[0];
        var path = firstLine[1];
        var headers = new Dictionary<string, string>();
        var body = "";
        var bodyIndex = Array.IndexOf(lines, "") + 1;
        if (bodyIndex < lines.Length)
        {
            body = lines[bodyIndex];
        }

        for (var i = 1; i < bodyIndex; i++)
        {
            var header = lines[i].Split(": ");
            if (header.Length == 2)
                headers.Add(header[0], header[1]);
        }

        return new HttpRequest(method, path, body, headers);
    }
}