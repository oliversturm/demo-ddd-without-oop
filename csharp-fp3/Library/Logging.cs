namespace CsharpFp3.Library;

public static class Logging
{
    public static T LogReturn<T>(string message, T r)
    {
        Console.WriteLine(message);
        return r;
    }

    public static Action<T> Output<T>(string src, string message) =>
        x =>
        {
            Console.WriteLine($"[{src}] {message} | {x}");
        };

    public static void OutputError<T>(string src, T error)
    {
        Console.Error.WriteLine($"\e[1;31m[{src} ERROR]\e[0m {error}");
    }
}
