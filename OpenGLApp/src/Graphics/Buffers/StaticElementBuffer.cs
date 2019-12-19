using System;
using System.Collections.Generic;
using System.Text;
using static CSGL.OpenGL;

namespace OpenGLApp.src.Graphics.Buffers
{
    class StaticElementBuffer : Intbuffer
    {
        public StaticElementBuffer(int[] array)
            :base(GL_ELEMENT_ARRAY_BUFFER, array, GL_STATIC_DRAW)
        {
        }
    }
}
