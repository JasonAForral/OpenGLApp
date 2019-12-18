using System;
using System.Collections.Generic;
using System.Text;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Buffers
{
    class VertexArray
    {
        private readonly uint id;
        public uint Id { get => id; }

        public VertexArray()
        {
            glGenVertexArrays(1, ref id);
        }

        public void Bind()
        {
            glBindVertexArray(Id);
        }
        public void Unbind()
        {
            glBindVertexArray(0);
        }
    }
}
