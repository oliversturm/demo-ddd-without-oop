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
        Indent(ObjectDumper.Dump(o, new DumpOptions() { DumpStyle = DumpStyle.CSharp }));

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
