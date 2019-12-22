using System;
using System.Collections.Generic;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Buffers
{
    public class StaticInterleavedVertexBuffer
    {
        readonly uint id;
        uint Id { get => id; }

        public StaticInterleavedVertexBuffer(List<Vertex> vertices)
        {
            float[] array = new float[vertices.Count * Vertex.SIZE];
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex vertex = vertices[i];
                vertex.ToArray().CopyTo(array, i * Vertex.SIZE);
            }
            glGenBuffers(1, ref id);
            glBindBuffer(GL_ARRAY_BUFFER, id);
            unsafe
            {
                fixed (void* vertexPtr = array)
                    glBufferData(GL_ARRAY_BUFFER, Vertex.SIZE, new IntPtr(vertexPtr), GL_STATIC_DRAW);

                uint vpos_location = 0;
                glEnableVertexAttribArray(vpos_location);
                glVertexAttribPointer(vpos_location, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 6, IntPtr.Zero);

                uint vnor_location = 0;
                glEnableVertexAttribArray(vnor_location);
                glVertexAttribPointer(vnor_location, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 6, IntPtr.Add(IntPtr.Zero, 3));
            }

            //uint index_buffer = 0;
            //glGenBuffers(1, ref index_buffer);
            //glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, index_buffer);
            //glBufferData(GL_ELEMENT_ARRAY_BUFFER, vertices.Count * sizeof(int), indices, GL_STATIC_DRAW);

            //arrayBuffer = new StaticArrayBuffer(array);

            //unsafe
            //{
            //    //glBufferSubData(GL_ARRAY_BUFFER, 0 * sizeof(float), 16 * sizeof(float), new IntPtr(&model.M11));
            //    //glBufferSubData(GL_ARRAY_BUFFER, 16 * sizeof(float), 16 * sizeof(float), new IntPtr(&view.M11));
            //    //glBufferSubData(GL_ARRAY_BUFFER, 32 * sizeof(float), 16 * sizeof(float), new IntPtr(&world.M11));
            //}
        }
    }
}
