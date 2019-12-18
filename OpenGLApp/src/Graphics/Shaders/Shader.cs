using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using static CSGL.CSGL;   // csgl*
using static CSGL.Glfw3;  // glfw*
using static CSGL.OpenGL; // gl*
using System.Runtime.InteropServices;

namespace OpenGLApp.src.Graphics.Shaders
{
    class Shader
    {
        public uint Id { get; }

        public Shader(uint shaderType, string shaderScript)
        {
            Id = glCreateShader(shaderType);
            IntPtr scriptPtr = Marshal.AllocHGlobal(shaderScript.Length);
            Marshal.Copy(Encoding.ASCII.GetBytes(shaderScript), 0, scriptPtr, shaderScript.Length);

            int length = shaderScript.Length;

            glShaderSource(Id, 1, ref scriptPtr, ref length);
            glCompileShader(Id);

            Marshal.FreeHGlobal(scriptPtr);

            int isCompiled = 0;

            glGetShaderiv(Id, GL_COMPILE_STATUS, ref isCompiled);

            if (isCompiled == 0)
            {
                int loglen = 0;
                glGetShaderiv(Id, GL_INFO_LOG_LENGTH, ref loglen);

                IntPtr log = Marshal.AllocHGlobal(loglen);

                glGetShaderInfoLog(Id, loglen, ref loglen, log);

                char[] chrs = new char[loglen];
                string logs = "";
                Marshal.Copy(log, chrs, 0, loglen);
                logs.Concat(chrs.AsEnumerable());
                Marshal.FreeHGlobal(log);
                Console.Error.WriteLine(logs);
                Id = 0;
            }
        }
    }
}
