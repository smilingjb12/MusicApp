using System.IO;

namespace Data
{
    public static class FileUtils
    {
        public static byte[] ReadBytesFromStream(Stream stream)
        {
            byte[] buffer = new byte[1024 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}