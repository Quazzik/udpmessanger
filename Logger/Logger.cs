namespace Logger;

public class Logger
{
    private FileStream _file;
    private StreamWriter _writer;

    public Logger(string path)
    {
        _file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        _writer = new StreamWriter(_file);
    }

    public async Task LogErrorAsync(string message)
    {
        var now = DateTime.Now;
        var log = $"[ERROR] {now:G} - {message}\n";
        await _writer.WriteAsync(log);
    }

    ~Logger()
    {
        _writer.Close();
        _file.Close();
    }
}