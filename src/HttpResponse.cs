﻿using System.Text;

namespace codecrafters_http_server;

public class HttpResponse
{
    public string Status { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }

    public HttpResponse(string status, string body, Dictionary<string, string> headers)
    {
        Status = status;
        Body = body;
        Headers = headers;
    }

    public HttpResponse(string status, string body)
    {
        Status = status;
        Body = body;
        Headers = new Dictionary<string, string>();
    }

    public byte[] SerializeResponse()
    {
        var contentLength = 0;
        if (!string.IsNullOrEmpty(Body))
        {
            contentLength = Encoding.ASCII.GetBytes(Body).Length;
        }

        Headers.Add("Content-Length", contentLength.ToString());
        var headers = string.Join("\r\n", Headers.Select(header => $"{header.Key}: {header.Value}"));
        var responseString = $"HTTP/1.1 {Status}\r\n{headers}\r\n\r\n{Body}";
        return Encoding.ASCII.GetBytes(responseString);
    }

    public static HttpResponse Ok(string body)
    {
        return new HttpResponse("200 OK", body, new Dictionary<string, string> { { "Content-Type", "text/plain" } });
    }

    public static HttpResponse NotFound()
    {
        return new HttpResponse("404 Not Found", "", new Dictionary<string, string>());
    }

    public static HttpResponse BadRequest()
    {
        return new HttpResponse("400 Bad Request", "", new Dictionary<string, string>());
    }

    public static HttpResponse Created(string body)
    {
        return new HttpResponse("201 Created", body);
    }

    public void AddHeader(string key, string value)
    {
        Headers.Add(key, value);
    }
}