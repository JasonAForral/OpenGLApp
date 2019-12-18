using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenGLApp.src.ReadWrite
{
    public static class ReadWriteFile
    {
        public static string LoadScript(string path)
        {
            if (!File.Exists(path))
                return null;
            return File.ReadAllText(path);
        }

        public static void LoadObj(string path)
        {
            throw new NotImplementedException($"Load Obj Not Implemented {path}");
        }
    }
}
