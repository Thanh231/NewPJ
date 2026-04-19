
using System;
using UnityEngine;

public class PlayerModelFile_text : IPlayerModelFile
{
    public string Extension => "ini";

    public void ReadModel(string filepath, BasePlayerModel model)
    {
        StaticUtils.OpenFileForRead(filepath, stream =>
        {
            var reader = new FileStream_textReader(stream);
            var version = 0;
            reader.ReadOrWriteInt(ref version, "version");
            model.ReadOrWrite(reader, version);
        });
    }

    public void WriteModel(string filepath, BasePlayerModel model)
    {
        //on windows, sometimes it returns error 1224 when open file
        //due to that file is being processed by antivirus or disk is full
        try
        {
            StaticUtils.OpenFileForWrite(filepath, stream =>
            {
                var writer = new FileStream_textWriter(stream);
                var version = model.ModelVersion;
                writer.ReadOrWriteInt(ref version, "version");
                model.ReadOrWrite(writer, version);
            });
        }
        catch (Exception e)
        {
            if (e.Message.Contains("Win32 IO returned 1224"))
            {
                Debug.LogException(e);
            }
            else
            {
                StaticUtils.RethrowException(e);
            }
        }
    }
}
