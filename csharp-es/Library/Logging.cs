using System.Text.Json;
using System.Text.Json.Serialization;

namespace CsharpEs.Library;

public static class Logging
{
    public static T LogReturn<T>(string message, T r)
    {
        Console.WriteLine(message);
        return r;
    }

    static string Indent(string text, int indent = 4)
    {
        var prefix = new string(' ', indent);

        return Environment.NewLine
            + string.Join(
                Environment.NewLine,
                text.Split(Environment.NewLine).Select(line => prefix + line)
            );
    }

    public static string Format(Object? o) =>
        o != null
            ? Indent(
                JsonSerializer.Serialize(
                    o,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters = { new JsonStringEnumConverter() },
                    }
                )
            )
            : "(null)";

    public static void Output(string src, string message, Object? x)
    {
        Console.WriteLine($"[{src}] {message} | {Format(x)}");
    }

    public static void Output(string src, string message)
    {
        Console.WriteLine($"[{src}] {message}");
    }

    public static Action<T> OutputDelegate<T>(string src, string message) =>
        x =>
        {
            Output(src, message, x);
        };

    public static void OutputError<T>(string src, T error)
    {
        Console.Error.WriteLine($"\e[1;31m[{src} ERROR]\e[0m {Format(error)}");
    }
}
