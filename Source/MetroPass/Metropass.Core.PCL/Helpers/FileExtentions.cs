using System;
using System.Diagnostics;
using System.IO;

public static class FileExtentions
{
    public static byte[] ToArray(this Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }

    public static int ReadBytes(this Stream source, byte[] bytesToRead)
    {
        if (source.Position >= source.Length)
        {
            return -1;
        }

        long position = source.Position;
        int count = bytesToRead.Length;
        try
        {
            var read = source.Read(bytesToRead, 0, count);
           return read;
     
        }
        catch (Exception e) 
        {
            Debug.WriteLine(e.ToString());
            bytesToRead = null;
        }
        return 0;
    }
}