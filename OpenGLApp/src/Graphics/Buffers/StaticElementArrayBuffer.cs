using System;
using System.Collections.Generic;
using System.Text;
using static CSGL.OpenGL;

namespace OpenGLApp.src.Graphics.Buffers
{
    class StaticElementArrayBuffer : Intbuffer
    {
        public StaticElementArrayBuffer(int[] array)
            : base(GL_ELEMENT_ARRAY_BUFFER, array, GL_STATIC_DRAW)
        {
        }
    }
}