
using System;
using System.IO;

public class StringStream : IDisposable
{
    private MemoryStream stream = new MemoryStream();
    private StreamWriter cacheWriter;

    public StringStream()
    {
    }
        
    public StringStream(string str)
    {
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write(str);
        streamWriter.Flush();
    }

    public StreamReader GetReader()
    {
        stream.Position = 0;
        return new StreamReader(stream);
    }

    public StreamWriter GetWriter()
    {
        cacheWriter = new StreamWriter(stream);
        return cacheWriter;
    }

    public string GetString()
    {
        cacheWriter?.Flush();

        stream.Position = 0;
        var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    public void Dispose()
    {
        stream.Close();
    }

    public void Close()
    {
        Dispose();
    }
}
