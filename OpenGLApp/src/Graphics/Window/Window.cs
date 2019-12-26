using System;
using static CSGL.CSGL;   // csgl*
using static CSGL.Glfw3;  // glfw*

namespace OpenGLApp.src.Graphics.Window
{
    class Window
    {
        public IntPtr Id { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Title { get; set; }
        public IntPtr Share { get; set; }
        public IntPtr Monitor { get; set; }

        public Window(int width, int height, string title, IntPtr monitor, IntPtr share)
        {
            this.Width = width;
            this.Height = height;
            this.Title = title;
            this.Monitor = monitor;
            this.Share = share;


            csglLoadGlfw();
            glfwInit();
            Id = glfwCreateWindow(width, height, title, monitor: NULL,share: NULL);
            if (Id == null)
                return;
            glfwMakeContextCurrent(Id);
            csglLoadGL();
#if false
            int[] extensionCount = { 0 };
            glGetIntegerv(GL_NUM_EXTENSIONS, extensionCount);
            Console.WriteLine($"Extensions: {extensionCount[0]}");

            for (uint i = 0; i < extensionCount[0]; ++i)
            {
                IntPtr extension = glGetStringi(GL_EXTENSIONS, i);
                StringBuilder n = new StringBuilder();
                unsafe
                {
                    byte* name = (byte*)extension.ToPointer();
                    do
                    {
                        n.Append((char)*name);
                    } while (*++name != 0);

                    Console.WriteLine($"{i} : {n}");
                }
            }
#endif
        }
    }
}
