using System;
using System.Collections.Generic;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Buffers
{
    public class StaticInterleavedVertexBuffer
    {
        readonly uint id;
        uint Id { get => id; }

        public StaticInterleavedVertexBuffer(List<Vertex> vertexList)
        {
            float[] vertexArray = ToArray(vertexList);

            glGenBuffers(1, ref id);
            glBindBuffer(GL_ARRAY_BUFFER, id);
            unsafe
            {
                fixed (void* vertexPtr = vertexArray)
                    glBufferData(GL_ARRAY_BUFFER, vertexArray.Length * sizeof(float), new IntPtr(vertexPtr), GL_STATIC_DRAW);
            }
        }

        public static float[] ToArray(List<Vertex> vertexList)
        {
            float[] vertexArray = new float[vertexList.Count * Vertex.SIZE];
            for (int i = 0; i < vertexList.Count; i++)
            {
                _ = vertexList[i].CopyTo(vertexArray, i * Vertex.SIZE);
            }
            return vertexArray;
        }
    }
}
