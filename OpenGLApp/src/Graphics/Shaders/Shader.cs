using System;
using System.Runtime.InteropServices;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Shaders
{
    class Shader
    {
        public uint Id { get; }

        public Shader(uint shaderType, string shaderScript)
        {
            Id = glCreateShader(shaderType);

            int length = shaderScript.Length;
            var intPtr = Marshal.StringToCoTaskMemAnsi(shaderScript);
            glShaderSource(Id, 1, ref intPtr, ref length);

            Marshal.FreeCoTaskMem(intPtr);

            glCompileShader(Id);

            int isCompiled = 0;

            glGetShaderiv(Id, GL_COMPILE_STATUS, ref isCompiled);

#if DEBUG
            Console.WriteLine($"Compiled: 0x{shaderType:x}\t{isCompiled}");
#endif

            if (isCompiled == 0)
            {
                glGetShaderiv(Id, GL_INFO_LOG_LENGTH, ref length);

                intPtr = Marshal.AllocCoTaskMem(length);
                glGetShaderInfoLog(Id, length, ref length, intPtr);

                Console.Error.WriteLine(Marshal.PtrToStringAnsi(intPtr));

                Marshal.FreeCoTaskMem(intPtr);
                Id = 0;
            }
        }
    }
}
