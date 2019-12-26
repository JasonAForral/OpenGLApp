using System;
using System.Collections.Generic;
using System.Linq;
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
            uint vertexShader = new Shader(GL_VERTEX_SHADER, vert).Id;
            uint fragShader = new Shader(GL_FRAGMENT_SHADER, frag).Id;

            glAttachShader(Id, vertexShader);
            glAttachShader(Id, fragShader);

            if (vertexShader == 0 || fragShader == 0)
            {
                Id = 0;
                return;
            }

            glLinkProgram(Id);

#if DEBUG
            glValidateProgram(Id);
#endif
            int isLinked = 0;

            glGetProgramiv(Id, GL_LINK_STATUS, ref isLinked);
            if (isLinked == 0)
            {
                int loglen = 0;
                glGetProgramiv(Id, GL_INFO_LOG_LENGTH, ref loglen);

                IntPtr log = Marshal.AllocHGlobal(loglen);

                glGetProgramInfoLog(Id, loglen, ref loglen, log);

                char[] chrs = new char[loglen];
                string logs = null;
                Marshal.Copy(log, chrs, 0, loglen);
                logs.Concat(chrs.AsEnumerable());
                Marshal.FreeHGlobal(log);
                Console.Error.WriteLine(logs);
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
