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
                _ = vertex.CopyTo(array, i * Vertex.SIZE);
            }
#if DEBUG
            foreach(var item in array)
            {
                Console.Write($"{item}, ");
            }
            Console.WriteLine();
#endif
            glGenBuffers(1, ref id);
            glBindBuffer(GL_ARRAY_BUFFER, id);
            unsafe
            {
                fixed (void* vertexPtr = array)
                    glBufferData(GL_ARRAY_BUFFER, array.Length * sizeof(float), new IntPtr(vertexPtr), GL_STATIC_DRAW);
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
