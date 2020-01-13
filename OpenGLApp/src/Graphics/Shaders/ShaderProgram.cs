using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Shaders
{
    class ShaderProgram
    {
        public uint Id { get; }

        private readonly Dictionary<string, uint> locations;

        public ShaderProgram(string vert, string frag)
        {
            locations = new Dictionary<string, uint>();
            Id = glCreateProgram();
            var vertexShader = new Shader(GL_VERTEX_SHADER, vert).Id;
            var fragShader = new Shader(GL_FRAGMENT_SHADER, frag).Id;

            glAttachShader(Id, vertexShader);
            glAttachShader(Id, fragShader);

            if (vertexShader == 0 || fragShader == 0)
            {
                Id = 0;
                return;
            }

            glLinkProgram(Id);

            int isLinked = 0;

            glGetProgramiv(Id, GL_LINK_STATUS, ref isLinked);
#if DEBUG
            glValidateProgram(Id);
            Console.WriteLine($"Linked: {isLinked}");
#endif

            if (isLinked == 0)
            {
                int length = 0;
                glGetProgramiv(Id, GL_INFO_LOG_LENGTH, ref length);

                var intPtr = Marshal.AllocHGlobal(length);
                glGetProgramInfoLog(Id, length, ref length, intPtr);
                Console.Error.WriteLine(Marshal.PtrToStringAnsi(intPtr));
                Marshal.FreeHGlobal(intPtr);
                Id = 0;
                return;
            }
        }

        public void Bind()
        {
            glUseProgram(Id);
        }

        public void Unbind()
        {
            glUseProgram(0);
        }

        internal uint GetLocation(string key)
        {
            if (!locations.ContainsKey(key))
            {
                locations.Add(key, (uint)glGetAttribLocation(Id, key));
            }
            return locations[key];
        }
    }
}
