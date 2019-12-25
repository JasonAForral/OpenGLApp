using System;
using static CSGL.OpenGL; // gl*	

namespace OpenGLApp.src.Graphics.Buffers
{
    class Buffer
    {
        protected readonly uint id;
        protected readonly uint target;
        public uint Id => id;


        public Buffer(uint target)
        {
            this.target = target;
            glGenBuffers(1, ref id);
            glBindBuffer(target, id);
        }


        public void Bind()
        {
            glBindBuffer(target, id);
        }
        public void Unbind()
        {
            glBindBuffer(target, 0);
        }
    }

    class FloatBuffer : Buffer
    {
        public FloatBuffer(uint target, float[] array, uint usage)
            : base(target)
        {
            unsafe
            {
                fixed (void* vertexPtr = array)
                {
                    glBufferData(target, array.Length * sizeof(float), new IntPtr(vertexPtr), usage);
                }
            }
        }
    }
    class Intbuffer : Buffer
    {
        public Intbuffer(uint target, int[] array, uint usage)
            : base(target)
        {
            unsafe
            {
                fixed (void* arrayPtr = array)
                {
                    glBufferData(target, array.Length * sizeof(float), new IntPtr(arrayPtr), usage);
                }
            }
        }
    }

}