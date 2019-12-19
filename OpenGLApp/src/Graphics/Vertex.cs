using System.Numerics;

namespace OpenGLApp.src.Graphics
{
    public struct Vertex
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 texCoord;
        public bool Equals(Vertex other)
        {
            return pos == other.pos && normal == other.normal && texCoord == other.texCoord;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex)
                return Equals((Vertex)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return ((pos.GetHashCode() ^ (normal.GetHashCode() << 1)) >> 1) ^ (texCoord.GetHashCode() << 1);
        }

        public static bool operator ==(Vertex a, Vertex b)
        {
            return a.pos == b.pos && a.normal == b.normal && a.texCoord == b.texCoord;
        }

        public static bool operator !=(Vertex a, Vertex b)
        {
            return a.pos != b.pos || a.normal != b.normal || a.texCoord != b.texCoord;
        }
    }


}
