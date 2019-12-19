using System;
using System.Collections.Generic;
using System.Text;
using static CSGL.OpenGL;

namespace OpenGLApp.src.Graphics.Buffers
{
    class StaticArrayBuffer : FloatBuffer
    {
        public StaticArrayBuffer(float[] array)
            :base(GL_ARRAY_BUFFER, array, GL_STATIC_DRAW)
        {
        }
    }
}
