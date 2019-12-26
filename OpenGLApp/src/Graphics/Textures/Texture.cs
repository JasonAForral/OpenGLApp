using System;
using System.IO;
using System.Text;
using static CSGL.OpenGL; // gl*

namespace OpenGLApp.src.Graphics.Textures
{
    class Texture
    {
        private byte[] raw;

        public byte[] Data { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Id { get; private set; }

        public uint Format { get; private set; }
        private byte[] palette;

        public Texture(string path)
        {
            if (ReadFile(path) < 0)
                return;

            int candidate = (raw[0] << 8) + raw[1];
            Console.WriteLine($"Checking: {candidate:x4}");
            switch (candidate)
            {
                case 0x8950:
                    Console.WriteLine("PNG detected");
                    ParsePNG(null);
                    break;
                case 0x424d:
                    Console.WriteLine("BMP detected");
                    ParseBMP(null);
                    break;
                default:
                    Console.WriteLine("No type detected");
                    return;
            }

            uint texture = 0;
            glGenTextures(1, ref texture);
            Console.WriteLine($"Texture ID: {texture}");
            glBindTexture(GL_TEXTURE_2D, texture);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, (int)GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, (int)GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);

            unsafe
            {
                fixed (byte* dataPtr = Data)
                {
                    glTexImage2D(GL_TEXTURE_2D, 0, (int)Format, Width, Height, 0, Format, GL_UNSIGNED_BYTE, new IntPtr(dataPtr));
                }
            }
        }

        public Texture ParseBMP(string path)
        {

            if (path != null)
            {
                if (ReadFile(path) < 0)
                    return this;
            }

            int offset;
            int bitsPerPixel;

            unsafe
            {
                fixed (byte* head = raw)
                {
                    byte* ptr = head + 0x9;
                    offset = (*++ptr) | (*++ptr << 8) | (*++ptr << 16) | (*++ptr << 24);
                    ptr += 4;
                    Width = (*++ptr) | (*++ptr << 8) | (*++ptr << 16) | (*++ptr << 24);
                    Height = (*++ptr) | (*++ptr << 8) | (*++ptr << 16) | (*++ptr << 24);
                    ptr += 2;
                    bitsPerPixel = (*++ptr) | (*++ptr << 8);
                }
            }
            Format = bitsPerPixel switch
            {
                24 => GL_RGB,
                _ => GL_RGBA,
            };
            Data = new byte[raw.Length - offset];
            Array.Copy(raw, offset, Data, 0, Data.Length);

#if DEBUG
            Console.WriteLine($"offset: {offset}");
            Console.WriteLine($"dim: {Width} x {Height}");
            Console.WriteLine($"Length: {Data.Length}");

#endif
            return this;
        }

        public Texture ParsePNG(string path)
        {
            byte bitDepth;
            byte colorType;
            byte compMethod;
            byte filterMethod;
            byte interlaceMethod;

            int ppuX = 0;
            int ppuY = 0;
            byte unitSpecifier = 0;

            if (path != null)
            {
                if (ReadFile(path) < 0)
                    return this;
            }

            byte[] header = { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a };


            for (int i = 0; i < header.Length; ++i)
            {
                if (header[i] != raw[i])
                {
                    Console.WriteLine("Failure to read PNG");
                    return this;
                }
            }
            Console.WriteLine($"{(char)raw[1]}{(char)raw[2]}{(char)raw[3]}");

            unsafe
            {
                fixed (byte* head = raw)
                {
                    byte* ptr = head + 7;


                    byte* end = head + raw.Length;
                    while (ptr < end)
                    {
                        byte* start = ptr;
                        int length = (*++ptr << 24) + (*++ptr << 16) + (*++ptr << 8) + *++ptr;
                        StringBuilder chunkType = new StringBuilder();
                        chunkType.Append((char)*++ptr);
                        chunkType.Append((char)*++ptr);
                        chunkType.Append((char)*++ptr);
                        chunkType.Append((char)*++ptr);
                        switch (chunkType.ToString())
                        {
                            case "IHDR":
                                Console.WriteLine(" IHDR");
                                Width = (*++ptr << 24) + (*++ptr << 16) + (*++ptr << 8) + *++ptr;
                                Height = (*++ptr << 24) + (*++ptr << 16) + (*++ptr << 8) + *++ptr;
                                bitDepth = *++ptr; 
                                colorType = *++ptr; 
                                compMethod = *++ptr; 
                                filterMethod = *++ptr; 
                                interlaceMethod = *++ptr; 
                                Console.WriteLine($" width: {Width}, height: {Height}");
                                Console.WriteLine($"bit {bitDepth}, color {colorType}, comp {compMethod}, filter {filterMethod}, interlaced {interlaceMethod}");
                                ptr += 4;
                                int arrayLength = Width * Height * colorType switch
                                {
                                    0 => bitDepth,
                                    2 => 3 * bitDepth,
                                    3 => bitDepth,
                                    4 => 2 * bitDepth,
                                    6 => 4 * bitDepth,
                                    _ => throw new Exception("Error: Invalid color type."),
                                };

                                Data = new byte[arrayLength];

                                break;
                            case "PLTE":
                                Console.WriteLine(" PLTE");
                                if (length % 3f != 0)
                                    throw new Exception("Palette bytes is divisible by 3");

                                palette = new byte[length];
                                //for (int i = 0; i < length; ++i)
                                //{
                                //    palette[i] = *++ptr;
                                //}
                                Array.Copy(raw, ptr - head + 1, palette, 0, length);
                                //for (int i = 0; i < length; ++i)
                                //    Console.WriteLine($"{palette[i]} ");
                                ptr += length;
                                ptr += 4;
                                break;
                            case "IDAT":
                                Console.WriteLine(" IDAT");
                                Console.WriteLine($"length: {length}");
                                ptr += length;
                                ptr += 4;
                                break;
                            case "IEND":
                                Console.WriteLine(" IEND");
                                ptr += 4;
                                ptr = end;
                                break;
                            case "pHYs":
                                Console.WriteLine(" pHYs");
                                ppuX = (*++ptr << 24) + (*++ptr << 16) + (*++ptr << 8) + *++ptr;
                                ppuY = (*++ptr << 24) + (*++ptr << 16) + (*++ptr << 8) + *++ptr;
                                unitSpecifier = *++ptr;
                                ptr += 4;
                                Console.WriteLine($"PPU {ppuX}, {ppuY} : {unitSpecifier}");
                                break;
                            default:
                                Console.WriteLine($" {chunkType.ToString()} is unknown");
                                ptr += length + 4;
                                break;
                        }
                    }
                }
            }

            Console.WriteLine(Data.Length);

            return this;
        }

        int ReadFile(string path)
        {
            if (!File.Exists(path))
                return -1;
            raw = File.ReadAllBytes(path);
            return 0;
        }
    }


}
