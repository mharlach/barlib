using System;
using RestSharp;
using BarLib;

public class Uploader
{
    private string url;

    public Uploader(string url)
    {
        this.url = url;
    }

    public void Put(string path, ModelBase item) => Execute(Method.PUT, path, item);

    public void Post(string path, ModelBase item) => Execute(Method.POST, path, item);

    private void Execute(Method method, string path, ModelBase item)
    {
        Console.Write($"[{method}] -- {item.Name}");
        var client = new RestClient(url);
        var request = new RestRequest
        {
            Method = method,
            Resource = path,
        };
        request.AddJsonBody(item);

        var response = client.Execute(request);

        Console.Write($" -- {response.StatusCode}\r\n");
    }
}