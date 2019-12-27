using System;
using System.Text;
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

            var bytes = Encoding.ASCII.GetBytes(shaderScript);

            unsafe
            {
                fixed(void* ptr = bytes)
                {
                    var intPtr = new IntPtr(ptr);
                    glShaderSource(Id, 1, ref intPtr, ref length);
                }
            }

            glCompileShader(Id);

            int isCompiled = 0;

            glGetShaderiv(Id, GL_COMPILE_STATUS, ref isCompiled);

#if DEBUG
            Console.WriteLine($"Compiled: 0x{shaderType:x}\t{isCompiled}");
#endif

            if (isCompiled == 0)
            {
                glGetShaderiv(Id, GL_INFO_LOG_LENGTH, ref length);

                bytes = new byte[length];
                unsafe
                {
                    fixed (void* ptr = bytes)
                    {
                        var intPtr = new IntPtr(ptr);
                        glGetShaderInfoLog(Id, length, ref length, intPtr);
                    }
                }
                Console.Error.WriteLine(Encoding.ASCII.GetString(bytes));
                Id = 0;
            }
        }
    }
}
