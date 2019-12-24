using System;
using System.Numerics;


namespace OpenGLApp.src.Graphics
{
    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;

        public const int SIZE = 8;
        public const int SIZE_BYTES = SIZE * sizeof(float);

        public Vertex(float x, float y, float z, float nx, float ny, float nz, float u, float v)
        {
            position = new Vector3(x, y, z);
            normal = new Vector3(nx, ny, nz);
            texCoord = new Vector2(u, v);
#if DEBUG
            Console.WriteLine($"{position.ToString()}\t{normal.ToString()}\t{texCoord.ToString()}");
#endif

        }

        internal float[] CopyTo(float[] array, int index)
        {
            position.CopyTo(array, index + 0);
            normal.CopyTo(array, index + 3);
            texCoord.CopyTo(array, index + 6);
            return array;
        }

        public float[] ToArray()
        {
            float[] array = new float[SIZE];
            position.CopyTo(array, 0);
            normal.CopyTo(array, 3);
            texCoord.CopyTo(array, 6);
            return null;
        }

        public bool Equals(Vertex vertex)
        {
            return position == vertex.position && texCoord == vertex.texCoord && normal == vertex.normal;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex)
                return Equals((Vertex)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return ((position.GetHashCode() ^ (normal.GetHashCode() << 1)) >> 1) ^ (texCoord.GetHashCode() << 1);
        }
        public static bool operator ==(Vertex a, Vertex b)
        {
            return a.position == b.position && a.normal == b.normal && a.texCoord == b.texCoord;
        }

        public static bool operator !=(Vertex a, Vertex b)
        {
            return a.position != b.position || a.normal != b.normal || a.texCoord != b.texCoord;
        }
    }
}
